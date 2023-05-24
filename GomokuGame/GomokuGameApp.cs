using FlaskSharp;
using LibGomokuGame;
using LibGomokuGame.Models;

namespace GomokuGame
{
    public class GomokuGameApp : FlaskApp
    {
        private readonly object Lock = new object();

        public List<GomokuSession> AllSessions { get; } = new List<GomokuSession>();
        public List<string> AllPlayers { get; } = new List<string>();




        [FlaskRoute("/register")]
        public NetResult<RegisterData> Register()
        {
            return NetResult<RegisterData>.Ok(
                new RegisterData()
                {
                    Name = Guid.NewGuid().ToString()
                });
        }

        [FlaskRoute("/pairme")]
        public NetResult<PairMeData> PairMe(string player)
        {
            // 保证只有一个线程在进行配对操作
            lock (Lock)
            {
                GomokuSession? joinedSession =
                    AllSessions.FirstOrDefault(_session => _session.HostPlayer == player || _session.GuestPlayer == player);

                if (joinedSession != null)
                    return NetResult<PairMeData>.Ok(
                        new PairMeData()
                        {
                            Session = joinedSession
                        });

                // ------ 没有已经配对好的会话, 下面是执行配对 ------




                // 查找等待配对的游戏会话
                GomokuSession? sessionWaitForPair =
                        AllSessions.FirstOrDefault(session => session.State == GomokuSessionState.Wait);

                if (sessionWaitForPair != null)
                {
                    // 进行配对
                    sessionWaitForPair.State = GomokuSessionState.Play;
                    sessionWaitForPair.GuestPlayer = player;

                    return NetResult<PairMeData>.Ok(
                        new PairMeData()
                        {
                            Session = sessionWaitForPair,
                        });
                }

                // 没人等着陪他玩, 只能新建游戏会话
                GomokuSession newWaitSession = new GomokuSession()
                {
                    ID = Guid.NewGuid().ToString(),
                    State = GomokuSessionState.Wait,
                    HostPlayer = player,
                };

                // 保存
                AllSessions.Add(newWaitSession);

                return NetResult<PairMeData>.Ok(
                    new PairMeData()
                    {
                        Session = newWaitSession
                    });
            }
        }

        [FlaskRoute("/mymove")]
        public NetResult<MyMoveData> MyMove(string player, string id, int move)
        {
            GomokuSession? joinedSession =
                    AllSessions.FirstOrDefault(_session => _session.HostPlayer == player || _session.GuestPlayer == player);

            // 判断是否已经加入会话
            if (joinedSession == null)
                return NetResult<MyMoveData>.Err(1, "Player has not joined a game session");

            // 判断玩家类型
            if (joinedSession.HostPlayer == player)
            {
                // 判断回合
                if (!joinedSession.HostPlayerNeetMove)
                    return NetResult<MyMoveData>.Err(2, "Not your turn");

                joinedSession.HostPlayerLastStep = move;
            }
            else
            {
                // 判断回合
                if (joinedSession.HostPlayerNeetMove)
                    return NetResult<MyMoveData>.Err(2, "Not your turn");

                joinedSession.GuestPlayerLastStep = move;
            }

            // 移动之后, 切换会话的回合状态
            joinedSession.HostPlayerNeetMove ^= true;

            return NetResult<MyMoveData>.Ok(new MyMoveData());
        }

        [FlaskRoute("/theirmove")]
        public NetResult<TheirMoveData> TheirMove(string player, string id)
        {
            GomokuSession? joinedSession =
                    AllSessions.FirstOrDefault(_session => _session.HostPlayer == player || _session.GuestPlayer == player);

            // 判断是否已经加入会话
            if (joinedSession == null)
                return NetResult<TheirMoveData>.Err(1, "Player has not joined a game session");

            // 判断玩家类型
            if (joinedSession.HostPlayer == player)
            {
                // 判断回合
                if (!joinedSession.HostPlayerNeetMove)
                    return NetResult<TheirMoveData>.Err(2, "The opponent's round is not over yet");

                // 判断对方是否已经下棋
                int step = joinedSession.GuestPlayerLastStep;
                if (step == -1)
                    return NetResult<TheirMoveData>.Err(3, "The opponent has not started any rounds yet");

                return NetResult<TheirMoveData>.Ok(
                    new TheirMoveData()
                    {
                        Step = step
                    });
            }
            else
            {
                // 判断回合
                if (joinedSession.HostPlayerNeetMove)
                    return NetResult<TheirMoveData>.Err(2, "Not your turn");

                // 判断对方是否已经下棋 (因为当前玩家是客, 轮到客的时候, 主已经下棋了, 所以这种情况不存在, 但是还是写上吧)
                int step = joinedSession.HostPlayerLastStep;
                if (step == -1)
                    return NetResult<TheirMoveData>.Err(3, "The opponent has not started any rounds yet");

                return NetResult<TheirMoveData>.Ok(
                    new TheirMoveData()
                    {
                        Step = step
                    });
            }
        }

        [FlaskRoute("/quit")]
        public NetResult<QuitData> Quit(string player, string id)
        {
            GomokuSession? session =
                AllSessions.FirstOrDefault(_session => _session.ID == id);

            if (session == null)
                return NetResult<QuitData>.Err(1, "Session is not exist");

            if (session.HostPlayer != player &&
                session.GuestPlayer != player)
                return NetResult<QuitData>.Err(2, "Player is not in the specified session");

            AllSessions.Remove(session);

            return NetResult<QuitData>.Ok(new QuitData());
        }
    }
}