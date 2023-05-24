using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibGomokuGame.Models;

namespace WpfGomokuGameClient.Services
{
    [ObservableObject]
    public partial class GomokuService : DispatcherObject
    {
        private static TimeSpan clientTimeout = TimeSpan.FromSeconds(30);


        private string? playerId;
        private string? sessionId;
        private bool canMove;

        public string? PlayerId => playerId;

        public GomokuService(
            NotifyService notifyService)
        {
            NotifyService = notifyService;
        }

        private HttpClient client = new HttpClient()
        {
            Timeout = clientTimeout
        };

        [ObservableProperty]
        private ObservableCollection<int> gomokuBoard =
            new ObservableCollection<int>(Enumerable.Repeat(0, 15 * 15));

        public void UpdateGomokuServer(Uri baseAddress)
        {
            client = new HttpClient()
            {
                Timeout = clientTimeout,
                BaseAddress = baseAddress
            };
        }

        public async Task RegisterAsync()
        {
            // 注册
            NetResult<RegisterData>? registerResult = await client.GetFromJsonAsync<NetResult<RegisterData>>("/register");

            if (registerResult == null)
            {
                MessageBox.Show("Can't register on server", "Network issue", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!registerResult.IsSuccess)
            {
                MessageBox.Show($"Code: {registerResult.ErrorCode}, Message: {registerResult.Message}", "Register failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SetProperty(ref playerId, registerResult.Data.Name, nameof(PlayerId));
        }

        public async Task GameAsync()
        {
            bool win, lose;

            if (playerId == null)
            {
                await RegisterAsync();

                if (playerId == null)
                    return;
            }

            // 配对并等待游戏准备完成
            NetResult<PairMeData>? pairMeResult;

            while (true)
            {
                pairMeResult = await client.GetFromJsonAsync<NetResult<PairMeData>>(
                    $"/pairme?player={Uri.EscapeDataString(playerId)}");

                if (pairMeResult == null)
                {
                    MessageBox.Show("Can't pair on server", "Network issue", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!pairMeResult.IsSuccess)
                {
                    MessageBox.Show($"Code: {pairMeResult.ErrorCode}, Message: {pairMeResult.Message}", "Pair failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (pairMeResult.Data.Session.State == LibGomokuGame.GomokuSessionState.Play)
                    break;

                // 轮询间隔, 200ms
                await Task.Delay(20);
            }

            sessionId = pairMeResult.Data.Session.ID;

            // 如果游戏最开始就是自己的回合, 则等待输入并发送

            if (pairMeResult.Data.Session.HostPlayer == playerId && pairMeResult.Data.Session.HostPlayerNeetMove ||
                pairMeResult.Data.Session.GuestPlayer == playerId && !pairMeResult.Data.Session.HostPlayerNeetMove)
            {

                NotifyService.Text = "Your turn";


                int move = await WaitForMove();
                NetResult<MyMoveData>? myMoveResult = await client.GetFromJsonAsync<NetResult<MyMoveData>>(
                    $"/mymove?player={Uri.EscapeDataString(playerId)}&id={Uri.EscapeDataString(sessionId)}&move={move}");

                if (myMoveResult == null)
                {
                    MessageBox.Show("Failed to send movement to server", "Network issue", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (myMoveResult.ErrorCode == 1)
                {
                    MessageBox.Show("Game is already closed", "Game closed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ApplySelfMovement(move);
            }

            // 游戏循环
            while (true)
            {
                NotifyService.Text = "Waiting for the opponent";

                NetResult<TheirMoveData>? theirMoveData;

                while (true)
                {
                    theirMoveData = await client.GetFromJsonAsync<NetResult<TheirMoveData>>(
                        $"/theirmove?player={Uri.EscapeDataString(playerId)}&id={Uri.EscapeDataString(sessionId)}");

                    if (theirMoveData == null)
                    {
                        MessageBox.Show("Failed to get movement from server", "Network issue", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (theirMoveData.ErrorCode == 1)
                    {
                        MessageBox.Show("Game is already closed", "Game closed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (theirMoveData.IsSuccess)
                        break;

                    // 轮询间隔 200ms
                    await Task.Delay(20);
                }

                ApplyOpponentMovement(theirMoveData.Data.Step);
                CheckGomokuResult(out win, out lose);
                if (PromptGomokuResult(win, lose))
                {
                    Dispatcher.Invoke(Application.Current.Shutdown);
                    return;
                }

                NotifyService.Text = "Your turn";

                int move = await WaitForMove();
                NetResult<MyMoveData>? myMoveResult = await client.GetFromJsonAsync<NetResult<MyMoveData>>(
                    $"/mymove?player={Uri.EscapeDataString(playerId)}&id={Uri.EscapeDataString(sessionId)}&move={move}");

                if (myMoveResult == null)
                {
                    MessageBox.Show("Failed to send movement to server", "Network issue", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (myMoveResult.ErrorCode == 1)
                {
                    MessageBox.Show("Game is already closed", "Game closed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ApplySelfMovement(move);
                CheckGomokuResult(out win, out lose);
                if (PromptGomokuResult(win, lose))
                {
                    Dispatcher.Invoke(Application.Current.Shutdown);
                    return;
                }
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(
            nameof(MoveCommand))]
        TaskCompletionSource<int>? moveTaskResult;

        public NotifyService NotifyService { get; }

        [RelayCommand(
            CanExecute = nameof(CanMove))]
        public void Move(int index)
        {
            if (moveTaskResult == null)
                return;

            moveTaskResult.SetResult(index);
        }

        [RelayCommand]
        public async Task Quit()
        {
            if (playerId == null ||
                sessionId == null)
            {
                MessageBox.Show("Game is not started", "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await client.GetAsync(
                $"/quit?player={Uri.EscapeDataString(playerId)}&id={Uri.EscapeDataString(sessionId)}");

            Application.Current.Shutdown();
        }

        public bool CanMove()
        {
            return moveTaskResult != null;
        }

        public async Task<int> WaitForMove()
        {
            MoveTaskResult = new TaskCompletionSource<int>();

            int result = await MoveTaskResult.Task;
            MoveTaskResult = null;

            return result;
        }

        public int GetGomokuValue(int x, int y)
        {
            return GomokuBoard[y * 15 + x];
        }

        public void CheckGomokuResult(out bool win, out bool lose)
        {
            bool CheckGomoku(int x, int y, int value)
            {
                int leftOffset = 0;
                int rightOffset = 0;
                int topOffset = 0;
                int bottomOffset = 0;
                int leftTopOffset = 0;
                int leftBottomOffset = 0;
                int rightTopOffset = 0;
                int rightBottomOffset = 0;

                // 向左
                for (int _leftOffset = -1; x + _leftOffset >= 0; _leftOffset--)
                {
                    if (value != GetGomokuValue(x + _leftOffset, y))
                        break;
                    leftOffset = _leftOffset;
                }

                // 向右
                for (int _rightOffset = 1; x + _rightOffset < 15; _rightOffset++)
                {
                    if (value != GetGomokuValue(x + _rightOffset, y))
                        break;
                    rightOffset = _rightOffset;
                }

                // 向上
                for (int _topOffset = -1; y + _topOffset >= 0; _topOffset--)
                {
                    if (value != GetGomokuValue(x, y + _topOffset))
                        break;
                    topOffset = _topOffset;
                }

                // 向下
                for (int _bottomOffset = 1; y + _bottomOffset < 15; _bottomOffset++)
                {
                    if (value != GetGomokuValue(x, y + _bottomOffset))
                        break;
                    bottomOffset = _bottomOffset;
                }

                // 左上
                for (int _leftTopOffset = -1; x + _leftTopOffset >= 0 && y + _leftTopOffset >= 0; _leftTopOffset--)
                {
                    if (value != GetGomokuValue(x + _leftTopOffset, y + _leftTopOffset))
                        break;
                    leftTopOffset = _leftTopOffset;
                }

                // 左下
                for (int _leftBottomOffset = -1; x + _leftBottomOffset >= 0 && y - _leftBottomOffset < 15; _leftBottomOffset--)
                {
                    if (value != GetGomokuValue(x + _leftBottomOffset, y - _leftBottomOffset))
                        break;
                    leftBottomOffset = _leftBottomOffset;
                }

                // 右上
                for (int _rightTopOffset = 1; x + _rightTopOffset < 15 && y - _rightTopOffset >= 0; _rightTopOffset++)
                {
                    if (value != GetGomokuValue(x + _rightTopOffset, y - _rightTopOffset))
                        break;
                    rightTopOffset = _rightTopOffset;
                }

                // 右下
                for (int _rightBottomOffset = 1; x + _rightBottomOffset < 15 && y + _rightBottomOffset < 15; _rightBottomOffset++)
                {
                    if (value != GetGomokuValue(x + _rightBottomOffset, y + _rightBottomOffset))
                        break;
                    rightBottomOffset = _rightBottomOffset;
                }

                return
                    rightOffset - leftOffset >= 4 ||
                    bottomOffset - topOffset >= 4 ||
                    rightBottomOffset - leftTopOffset >= 4 ||
                    rightTopOffset - leftBottomOffset >= 4;

            }


            for (int y = 0; y < 15; y++)
            {
                for (int x = 0; x < 15; x++)
                {
                    int value = GetGomokuValue(x, y);

                    if (CheckGomoku(x, y, value))
                    {
                        if (value == 1)
                        {
                            win = true;
                            lose = false;
                            return;
                        }
                        else if (value == 2)
                        {
                            win = false;
                            lose = true;
                            return;
                        }
                    }
                }
            }

            win = false;
            lose = false;
        }

        public bool PromptGomokuResult(bool win, bool lose)
        {
            if (win)
            {
                MessageBox.Show("You win", "You win", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            else if (lose)
            {
                MessageBox.Show("You lose", "You lose", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }

            return false;
        }

        public void ApplySelfMovement(int index)
        {
            Dispatcher.Invoke(() =>
            {
                GomokuBoard[index] = 1;
            });
        }

        public void ApplyOpponentMovement(int index)
        {
            Dispatcher.Invoke(() =>
            {
                GomokuBoard[index] = 2;
            });
        }
    }
}
