using UnityEngine;
using System.Collections;
using AVOSCloud;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AVOSCloudTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
  
	// Update is called once per frame
	void Update () {
       
	}
    private string msg1 =string.Empty;//注册用户
    private string msg2 = string.Empty;//登录用户
    private string msg3 = string.Empty;//添加游戏得分
    private string msg4 = string.Empty;//得分排行用户名top3
 
    void OnGUI()
    {
        //注册用户
        GUI.Label(new Rect(270, 50, 200, 80), msg1);
        if (GUI.Button(new Rect(50, 50, 200, 80), "注册"))
        {
            var userName = "xxz";
            var pwd = "xxz123";
            var user = new AVUser();
            user.Username = userName;
            user.Password = pwd;

            user.SignUpAsync().ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    msg1 = "注册成功";
                    var uid = user.ObjectId;
                }
                else
                {
                    msg1 = "注册失败";
                } 

            });
        }
        //用户登录
        GUI.Label(new Rect(600, 50, 200, 80), msg2);
        if (GUI.Button(new Rect(300, 50, 200, 80), "登录"))
        {
            var userName = "xxk";
            var pwd = "xxk123";
            AVUser.LogInAsync(userName, pwd).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    msg2 = t.Exception.Message; // 登录失败，可以查看错误信息。
                    msg2 = "登录失败";
                }
                else
                {
                    msg2 = "登陆成功";//登录成功
                }
            }); 
        }

        //添加游戏得分
        GUI.Label(new Rect(270, 200, 200, 80), msg3);
        if (GUI.Button(new Rect(50, 200, 200, 80), "添加gamescore"))
        {
            AVObject gameScore = new AVObject("GameScore");
            int newscore = 2000;
            string newplayername = "yly";

            gameScore["score"] = newscore;
            gameScore["playerName"] = newplayername;
        
            AVQuery<AVObject> query = new AVQuery<AVObject>("GameScore").WhereEqualTo("playerName", newplayername);

            query.CountAsync().ContinueWith(t =>
            {
                int count = t.Result;
                if (count >= 1)
                {
                    msg3 = "已经有了";
                    query.FindAsync().ContinueWith(t1 =>
                    {

                        Task task = Task.FromResult(0);
                        int cnt = 0;
                        foreach (AVObject result in t1.Result)
                        {
                            cnt = cnt + 1;
                            gameScore = result;
                            int val = gameScore.Get<int>("score");
                            if (val < newscore)
                                gameScore["score"] = newscore;
                            // 针对每一个AVObject,task都去调用SaveAsync这个方法。
                            task = task.ContinueWith(_ =>
                            {
                                // 返回一个Task。当保存完毕之后这个Task就会标记为完成。
                                return gameScore.SaveAsync();
                            });
                        };
                        msg3 = "得份录入成功";
                    });
                }                  
                else
                {
                    msg3 = "第一次出现";
                    gameScore.SaveAsync();
                }
                    
            });
        }


        //得份排行显示
        GUI.Label(new Rect(600, 200, 200, 80), msg4);
        if (GUI.Button(new Rect(300, 200, 200, 80), "排行显示"))
        {
            AVObject gameScore = new AVObject("GameScore");
            AVQuery<AVObject> query =AVObject.GetQuery("GameScore");
            query = query.OrderByDescending("score");

            query.FindAsync().ContinueWith(t =>
            {
                Task task = Task.FromResult(0);
                int cnt = 0;
                List<string> list_playerName = new List<string>();
                List<int> list_score = new List<int>();

                foreach (AVObject result in t.Result)
                {
                    
                    gameScore = result;
                    if (cnt <= 5)
                    {
                        cnt = cnt + 1;
                        list_playerName.Add(gameScore.Get<string>("playerName"));
                        list_score.Add(gameScore.Get<int>("score"));
                    } 
                }
                msg4 = cnt.ToString();
                string[] arr_playerName = list_playerName.ToArray();
                int[] arr_score = list_score.ToArray();
                for (; cnt > 1; cnt--)
                    msg4 = msg4 + arr_playerName[3 - cnt] +" " + arr_score[3 - cnt].ToString() + "\n";                                   
            });
        }

    }

}
