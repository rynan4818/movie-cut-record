using System;
using System.Data.SQLite;
using WebSocket4Net;
using Newtonsoft.Json;

namespace MovieCutRecord
{
    static class Constants
    {
        public const string dbFile = "beatsaber.db";
    }
    public class Beatsaber
    {
        public class Status
        {
            public class Game
            {
                public string pluginVersion { get; set; }   //プラグインの現在実行中のバージョン
                public string gameVersion { get; set; }     //現在のプラグインバージョンが対象としているゲームのバージョン
                public string scene { get; set; }           //プレイヤーの現在のアクティビティを示します
                public string mode { get; set; }
                public Game Clone()
                {
                    return (Game)MemberwiseClone();
                }
            }
            public class Beatmap
            {
                public string songName { get; set; }        //曲名
                public string songSubName { get; set; }     //曲のサブ名
                public string songAuthorName { get; set; }  //曲の作者名
                public string levelAuthorName { get; set; } //ビートマップ作成者名
                public string songHash { get; set; }        //ビートマップID
                public double songBPM { get; set; }         //BPM
                public double noteJumpSpeed { get; set; }   //ノーツピード
                public long songTimeOffset { get; set; }    //
                public string start { get; set; }           //
                public string paused { get; set; }          //
                public long length { get; set; }            //マップの長さ（ミリ秒）。曲の速度の乗数を調整しました。
                public string difficulty { get; set; }      //ビートマップの難易度
                public int notesCount { get; set; }         //マップキューブカウント
                public int bombsCount { get; set; }         //マップ爆弾カウント。 No Bombs修飾子が有効になっている場合でも設定します。
                public int obstaclesCount { get; set; }     //マップ障害カウント。 No Obstaclesモディファイヤを有効にしても設定します。
                public int maxScore { get; set; }           //修飾子の乗数でマップ上で取得可能な最大スコア
                public string maxRank { get; set; }         //現在の修飾子を使用して取得可能な最大ランク
                public string environmentName { get; set; }
                public Beatmap Clone()
                {
                    return (Beatmap)MemberwiseClone();
                }
            }
            public class Performance
            {
                public int score { get; set; }
                public int currentMaxScore { get; set; }
                public string rank { get; set; }
                public int passedNotes { get; set; }
                public int hitNotes { get; set; }
                public int missedNotes { get; set; }
                public int lastNoteScore { get; set; }
                public int passedBombs { get; set; }
                public int hitBombs { get; set; }
                public int combo { get; set; }
                public int maxCombo { get; set; }
                public Performance Clone()
                {
                    return (Performance)MemberwiseClone();
                }
            }
            public class Mod
            {
                public double multiplier { get; set; }
                public string obstacles { get; set; }
                public bool instaFail { get; set; }
                public bool noFail { get; set; }
                public bool batteryEnergy { get; set; }
                public bool disappearingArrows { get; set; }
                public bool noBombs { get; set; }
                public string songSpeed { get; set; }
                public double songSpeedMultiplier { get; set; }
                public bool noArrows { get; set; }
                public bool ghostNotes { get; set; }
                public bool failOnSaberClash { get; set; }
                public bool strictAngles { get; set; }
                public bool fastNotes { get; set; }
                public Mod Clone()
                {
                    return (Mod)MemberwiseClone();
                }
            }
            public class PlayerSettings
            {
                public bool staticLights { get; set; }
                public bool leftHanded { get; set; }
                public double playerHeight { get; set; }
                public bool reduceDebris { get; set; }
                public bool noHUD { get; set; }
                public bool advancedHUD { get; set; }
                public bool autoRestart { get; set; }
                public PlayerSettings Clone()
                {
                    return (PlayerSettings)MemberwiseClone();
                }
            }
            public Game game { get; set; } = new Game();
            public Beatmap beatmap { get; set; } = new Beatmap();
            public Performance performance { get; set; } = new Performance();
            public Mod mod { get; set; } = new Mod();
            public PlayerSettings playerSettings { get; set; } = new PlayerSettings();
        }

        [JsonProperty("event")]
        public string @event { get; set; }

        public long time { get; set; }
        public Status status { get; set; } = new Status();
    }
    class Program
    {
        Beatsaber bs = new Beatsaber();
        Beatsaber start_bs = new Beatsaber();
        Beatsaber end_bs = new Beatsaber();
        bool song = false;
        bool end = false;
        int pause = 0;
        string cleared = "";

        static void Main(string[] args)
        {
            Program obj = new Program();
            var ws = new WebSocket("ws://localhost:6557/socket");
            obj.DbCheck();
            bool loop_flag = true;
            bool open_run = false;

            /// 文字列受信
            ws.MessageReceived += (s, e) =>
            {
                obj.bs = JsonConvert.DeserializeObject<Beatsaber>(e.Message);
                obj.BeatsaberEvent();
            };

            /// バイナリ受信
            ws.DataReceived += (s, e) =>
            {
                Console.WriteLine("{0}:Binary Received Length:{1}", DateTime.Now.ToString(), e.Data.Length);
            };

            /// サーバ接続完了
            ws.Opened += (s, e) =>
            {
                Console.WriteLine("{0}:Server connected.", DateTime.Now.ToString());
            };

            /// エラー
            ws.Error += (s, e) =>
            {
                Console.WriteLine("{0}:Server error.", DateTime.Now.ToString());
            };

            /// クローズ
            ws.Closed += (s, e) =>
            {
                Console.WriteLine("{0}:Server closed.", DateTime.Now.ToString());
                if (loop_flag)
                {
                    open_run = true;
                    ws.Open();
                    open_run = false;
                }
            };

            /// サーバ接続開始
            ws.Open();

            Console.WriteLine("'END' of EXIT");

            /// 送受信ループ
            while (loop_flag)
            {
                var str = Console.ReadLine();
                if (str == "END")
                    loop_flag = false;

            }
            while (open_run) { }

            /// ソケットを閉じる
            ws.Close();
        }
        private void BeatsaberCopy(Beatsaber cp)
        {
            cp.@event = bs.@event;
            cp.time = bs.time;
            cp.status.game = bs.status.game.Clone();
            cp.status.beatmap = bs.status.beatmap.Clone();
            cp.status.performance = bs.status.performance.Clone();
            cp.status.mod = bs.status.mod.Clone();
            cp.status.playerSettings = bs.status.playerSettings.Clone();
        }
        private void BeatsaberEvent()
        {
            if (bs.@event == "songStart")
            {
                BeatsaberCopy(start_bs);
                song = true;
                end = false;
                pause = 0;
                cleared = bs.@event;
                Console.WriteLine("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", bs.@event, bs.time, bs.status.beatmap.songName, bs.status.beatmap.songSubName, bs.status.beatmap.songAuthorName, bs.status.beatmap.levelAuthorName, bs.status.beatmap.difficulty, bs.status.beatmap.notesCount, bs.status.beatmap.maxScore);
            }
            else if (bs.@event == "finished" || bs.@event == "failed")
            {
                BeatsaberCopy(end_bs);
                end = true;
                cleared = bs.@event;
                double scorePercentage;
                if (end_bs.status.performance.currentMaxScore == 0)
                    scorePercentage = 0.0;
                else
                    scorePercentage = double.Parse(String.Format("{0:F2}", ((double)end_bs.status.performance.score / (double)end_bs.status.performance.currentMaxScore) * 100.0));
                Console.WriteLine("{0}:{1}:{2}:{3}%:{4}:{5}:{6}", bs.@event, bs.time, bs.status.performance.score, scorePercentage, bs.status.performance.rank, bs.status.performance.missedNotes, bs.status.performance.maxCombo);
            }
            else if (bs.@event == "menu" || bs.@event == "pause" || bs.@event == "resume" || bs.@event == "hello")
            {
                Console.WriteLine("{0}:{1}", bs.@event, bs.time);
            }
            if (bs.@event == "menu")
            {
                if (song)
                {
                    int end_flag = 1;
                    if (end == false)
                        end_flag = 0;
                    SQLiteConnection db_con = new SQLiteConnection("Data Source=" + Constants.dbFile + ";Version=3;");
                    db_con.Open();
                    try
                    {
                        SQLiteCommand db_cmd = new SQLiteCommand(db_con);
                        db_cmd.CommandText = "insert into MovieCutRecord(startTime, endTime, menuTime, cleared, endFlag, pauseCount, pluginVersion," +
                                             "gameVersion, scene, mode, songName, songSubName, songAuthorName, levelAuthorName," +
                                             "songHash, songBPM, noteJumpSpeed, songTimeOffset, start, paused, length, difficulty," +"" +
                                             "notesCount, bombsCount, obstaclesCount, maxScore, maxRank," +
                                             "environmentName, scorePercentage, score, currentMaxScore, rank, passedNotes, hitNotes," +
                                             "missedNotes, lastNoteScore, passedBombs, hitBombs, combo, maxCombo, multiplier, obstacles," +
                                             "instaFail, noFail, batteryEnergy, disappearingArrows, noBombs, songSpeed, songSpeedMultiplier," +
                                             "noArrows, ghostNotes, failOnSaberClash, strictAngles, fastNotes, staticLights, leftHanded," +
                                             "playerHeight, reduceDebris, noHUD, advancedHUD, autoRestart) values (" +
                                             "@startTime, @endTime, @menuTime, @cleared, @endFlag, @pauseCount, @pluginVersion, @gameVersion, @scene, @mode," +
                                             "@songName, @songSubName, @songAuthorName, @levelAuthorName,@songHash, @songBPM, @noteJumpSpeed," +
                                             "@songTimeOffset, @start, @paused, @length, @difficulty, @notesCount," +
                                             "@bombsCount, @obstaclesCount, @maxScore, @maxRank, @environmentName, @scorePercentage, @score," +
                                             "@currentMaxScore, @rank, @passedNotes, @hitNotes, @missedNotes, @lastNoteScore, @passedBombs," +
                                             "@hitBombs, @combo, @maxCombo, @multiplier, @obstacles, @instaFail, @noFail, @batteryEnergy," +
                                             "@disappearingArrows, @noBombs, @songSpeed, @songSpeedMultiplier, @noArrows, @ghostNotes," +
                                             "@failOnSaberClash, @strictAngles, @fastNotes, @staticLights, @leftHanded, @playerHeight," +
                                             "@reduceDebris, @noHUD, @advancedHUD, @autoRestart)";
                        //独自カラム
                        db_cmd.Parameters.Add(new SQLiteParameter("@startTime", start_bs.time));
                        db_cmd.Parameters.Add(new SQLiteParameter("@endTime", end_bs.time));
                        db_cmd.Parameters.Add(new SQLiteParameter("@menuTime", bs.time));
                        db_cmd.Parameters.Add(new SQLiteParameter("@cleared", cleared));
                        db_cmd.Parameters.Add(new SQLiteParameter("@endFlag", end_flag));
                        db_cmd.Parameters.Add(new SQLiteParameter("@pauseCount", pause));
                        //gameステータス
                        db_cmd.Parameters.Add(new SQLiteParameter("@pluginVersion", start_bs.status.game.pluginVersion));
                        db_cmd.Parameters.Add(new SQLiteParameter("@gameVersion", start_bs.status.game.gameVersion));
                        db_cmd.Parameters.Add(new SQLiteParameter("@scene", start_bs.status.game.scene));
                        db_cmd.Parameters.Add(new SQLiteParameter("@mode", start_bs.status.game.mode));
                        //beatmapステータス
                        db_cmd.Parameters.Add(new SQLiteParameter("@songName", start_bs.status.beatmap.songName));
                        db_cmd.Parameters.Add(new SQLiteParameter("@songSubName", start_bs.status.beatmap.songSubName));
                        db_cmd.Parameters.Add(new SQLiteParameter("@songAuthorName", start_bs.status.beatmap.songAuthorName));
                        db_cmd.Parameters.Add(new SQLiteParameter("@levelAuthorName", start_bs.status.beatmap.levelAuthorName));
                        db_cmd.Parameters.Add(new SQLiteParameter("@length", start_bs.status.beatmap.length));
                        db_cmd.Parameters.Add(new SQLiteParameter("@songHash", start_bs.status.beatmap.songHash));
                        db_cmd.Parameters.Add(new SQLiteParameter("@songBPM", start_bs.status.beatmap.songBPM));
                        db_cmd.Parameters.Add(new SQLiteParameter("@noteJumpSpeed", start_bs.status.beatmap.noteJumpSpeed));
                        db_cmd.Parameters.Add(new SQLiteParameter("@songTimeOffset", start_bs.status.beatmap.songTimeOffset));
                        db_cmd.Parameters.Add(new SQLiteParameter("@start", start_bs.status.beatmap.start));
                        db_cmd.Parameters.Add(new SQLiteParameter("@paused", start_bs.status.beatmap.paused));
                        db_cmd.Parameters.Add(new SQLiteParameter("@difficulty", start_bs.status.beatmap.difficulty));
                        db_cmd.Parameters.Add(new SQLiteParameter("@notesCount", start_bs.status.beatmap.notesCount));
                        db_cmd.Parameters.Add(new SQLiteParameter("@bombsCount", start_bs.status.beatmap.bombsCount));
                        db_cmd.Parameters.Add(new SQLiteParameter("@obstaclesCount", start_bs.status.beatmap.obstaclesCount));
                        db_cmd.Parameters.Add(new SQLiteParameter("@maxScore", start_bs.status.beatmap.maxScore));
                        db_cmd.Parameters.Add(new SQLiteParameter("@maxRank", start_bs.status.beatmap.maxRank));
                        db_cmd.Parameters.Add(new SQLiteParameter("@environmentName", start_bs.status.beatmap.environmentName));
                        double scorePercentage;
                        if (end_bs.status.performance.currentMaxScore == 0)
                            scorePercentage = 0.0;
                        else
                            scorePercentage = double.Parse(String.Format("{0:F2}", ((double)end_bs.status.performance.score / (double)end_bs.status.performance.currentMaxScore) * 100.0));
                        db_cmd.Parameters.Add(new SQLiteParameter("@scorePercentage", scorePercentage));
                        //performanceステータス
                        db_cmd.Parameters.Add(new SQLiteParameter("@score", end_bs.status.performance.score));
                        db_cmd.Parameters.Add(new SQLiteParameter("@currentMaxScore", end_bs.status.performance.currentMaxScore));
                        db_cmd.Parameters.Add(new SQLiteParameter("@rank", end_bs.status.performance.rank));
                        db_cmd.Parameters.Add(new SQLiteParameter("@passedNotes", end_bs.status.performance.passedNotes));
                        db_cmd.Parameters.Add(new SQLiteParameter("@hitNotes", end_bs.status.performance.hitNotes));
                        db_cmd.Parameters.Add(new SQLiteParameter("@missedNotes", end_bs.status.performance.missedNotes));
                        db_cmd.Parameters.Add(new SQLiteParameter("@lastNoteScore", end_bs.status.performance.lastNoteScore));
                        db_cmd.Parameters.Add(new SQLiteParameter("@passedBombs", end_bs.status.performance.passedBombs));
                        db_cmd.Parameters.Add(new SQLiteParameter("@hitBombs", end_bs.status.performance.hitBombs));
                        db_cmd.Parameters.Add(new SQLiteParameter("@combo", end_bs.status.performance.combo));
                        db_cmd.Parameters.Add(new SQLiteParameter("@maxCombo", end_bs.status.performance.maxCombo));
                        //modステータス
                        db_cmd.Parameters.Add(new SQLiteParameter("@multiplier", start_bs.status.mod.multiplier));
                        db_cmd.Parameters.Add(new SQLiteParameter("@obstacles", start_bs.status.mod.obstacles));
                        db_cmd.Parameters.Add(new SQLiteParameter("@instaFail", start_bs.status.mod.instaFail));
                        db_cmd.Parameters.Add(new SQLiteParameter("@noFail", start_bs.status.mod.noFail));
                        db_cmd.Parameters.Add(new SQLiteParameter("@batteryEnergy", start_bs.status.mod.batteryEnergy));
                        db_cmd.Parameters.Add(new SQLiteParameter("@disappearingArrows", start_bs.status.mod.disappearingArrows));
                        db_cmd.Parameters.Add(new SQLiteParameter("@noBombs", start_bs.status.mod.noBombs));
                        db_cmd.Parameters.Add(new SQLiteParameter("@songSpeed", start_bs.status.mod.songSpeed));
                        db_cmd.Parameters.Add(new SQLiteParameter("@songSpeedMultiplier", start_bs.status.mod.songSpeedMultiplier));
                        db_cmd.Parameters.Add(new SQLiteParameter("@noArrows", start_bs.status.mod.noArrows));
                        db_cmd.Parameters.Add(new SQLiteParameter("@ghostNotes", start_bs.status.mod.ghostNotes));
                        db_cmd.Parameters.Add(new SQLiteParameter("@failOnSaberClash", start_bs.status.mod.failOnSaberClash));
                        db_cmd.Parameters.Add(new SQLiteParameter("@strictAngles", start_bs.status.mod.strictAngles));
                        db_cmd.Parameters.Add(new SQLiteParameter("@fastNotes", start_bs.status.mod.fastNotes));
                        //playerSettingsステータス
                        db_cmd.Parameters.Add(new SQLiteParameter("@staticLights", start_bs.status.playerSettings.staticLights));
                        db_cmd.Parameters.Add(new SQLiteParameter("@leftHanded", start_bs.status.playerSettings.leftHanded));
                        db_cmd.Parameters.Add(new SQLiteParameter("@playerHeight", start_bs.status.playerSettings.playerHeight));
                        db_cmd.Parameters.Add(new SQLiteParameter("@reduceDebris", start_bs.status.playerSettings.reduceDebris));
                        db_cmd.Parameters.Add(new SQLiteParameter("@noHUD", start_bs.status.playerSettings.noHUD));
                        db_cmd.Parameters.Add(new SQLiteParameter("@advancedHUD", start_bs.status.playerSettings.advancedHUD));
                        db_cmd.Parameters.Add(new SQLiteParameter("@autoRestart", start_bs.status.playerSettings.autoRestart));
                        db_cmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        db_con.Close();
                    }
                }
                song = false;
                end = false;
                pause = 0;
                cleared = "";
            }
            if (bs.@event == "resume" || bs.@event == "pause")
            {
                cleared = bs.@event;
                SQLiteConnection db_con = new SQLiteConnection("Data Source=" + Constants.dbFile + ";Version=3;");
                db_con.Open();
                try {
                    SQLiteCommand db_cmd = new SQLiteCommand(db_con);
                    db_cmd.CommandText = "insert into MovieCutPause(time, event) values (@time, @event)";
                    db_cmd.Parameters.Add(new SQLiteParameter("@time", bs.time));
                    db_cmd.Parameters.Add(new SQLiteParameter("@event", bs.@event));
                    db_cmd.ExecuteNonQuery();
                }
                finally
                {
                    db_con.Close();
                }
                if (bs.@event == "pause")
                {
                    ++pause;
                    BeatsaberCopy(end_bs);
                }
            }
        }
        private void DbCheck()
        {
            SQLiteConnection db_con = new SQLiteConnection("Data Source=" + Constants.dbFile + ";Version=3;");
            db_con.Open();
            try {
                SQLiteCommand db_cmd = new SQLiteCommand(db_con);
                //テーブル作成
                db_cmd.CommandText = "CREATE TABLE IF NOT EXISTS MovieCutRecord(" +
                    "startTime INTEGER NOT NULL PRIMARY KEY," +
                    "endTime INTEGER," +
                    "menuTime INTEGER NOT NULL," +
                    "cleared TEXT," +
                    "endFlag INTEGER NOT NULL," +
                    "pauseCount INTEGER NOT NULL," +
                    "pluginVersion TEXT," +
                    "gameVersion TEXT," +
                    "scene TEXT," +
                    "mode TEXT," +
                    "songName TEXT," +
                    "songSubName TEXT," +
                    "songAuthorName TEXT," +
                    "levelAuthorName TEXT," +
                    "songHash TEXT," +
                    "songBPM REAL," +
                    "noteJumpSpeed REAL," +
                    "songTimeOffset INTEGER," +
                    "start TEXT," +
                    "paused TEXT," +
                    "length INTEGER," +
                    "difficulty TEXT," +
                    "notesCount INTEGER," +
                    "bombsCount INTEGER," +
                    "obstaclesCount INTEGER," +
                    "maxScore INTEGER," +
                    "maxRank TEXT," +
                    "environmentName TEXT," +
                    "scorePercentage REAL," +
                    "score INTEGER," +
                    "currentMaxScore INTEGER," +
                    "rank TEXT," +
                    "passedNotes INTEGER," +
                    "hitNotes INTEGER," +
                    "missedNotes INTEGER," +
                    "lastNoteScore INTEGER," +
                    "passedBombs INTEGER," +
                    "hitBombs INTEGER," +
                    "combo INTEGER," +
                    "maxCombo INTEGER," +
                    "multiplier REAL," +
                    "obstacles TEXT," +
                    "instaFail INTEGER," +
                    "noFail INTEGER," +
                    "batteryEnergy INTEGER," +
                    "disappearingArrows INTEGER," +
                    "noBombs INTEGER," +
                    "songSpeed TEXT," +
                    "songSpeedMultiplier REAL," +
                    "noArrows INTEGER," +
                    "ghostNotes INTEGER," +
                    "failOnSaberClash INTEGER," +
                    "strictAngles INTEGER," +
                    "fastNotes INTEGER," +
                    "staticLights INTEGER," +
                    "leftHanded INTEGER," +
                    "playerHeight REAL," +
                    "reduceDebris INTEGER," +
                    "noHUD INTEGER," +
                    "advancedHUD INTEGER," +
                    "autoRestart INTEGER)";
                db_cmd.ExecuteNonQuery();
                db_cmd.CommandText = "CREATE TABLE IF NOT EXISTS MovieCutPause(" +
                    "time INTEGER NOT NULL PRIMARY KEY," +
                    "event TEXT)";
                db_cmd.ExecuteNonQuery();
            }
            finally {
                db_con.Close();
            }
        }
    }
}
