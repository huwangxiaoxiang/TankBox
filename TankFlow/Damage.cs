﻿using System;

namespace TankFlow
{
    public class Damage
    {
        public string source = "";
        public string victim = "";
        public int damage;//潜在伤害
        public int type;
        public int victimId;
        public int battleType;
        public int battleId;
        public bool valid = true;
        public bool friend = false;
        public int grade = 8;
        public int hitpart = 0;

        public string GetDamageType()
        {
            switch (type)
            {
                case 20:
                    return damage.ToString() + "(击毁)";
                case 1:
                    return damage.ToString();
                case 5:
                    return "未击穿";
                case 9:
                    return damage.ToString() + "(暴击)";
                case 17:
                    return "0";
                default:
                    return "";
            }
        }

        public string GetHitPartName()
        {
            switch (hitpart)
            {
                case 0:
                    return "车身正面";
                case 1:
                    return "车身侧面";
                case 2:
                    return "车身后面";
                case 3:
                    return "炮塔正面";
                case 4:
                    return "炮塔侧面";
                case 5:
                    return "炮塔后面";
                case 6:
                    return "炮塔正面";
                case 7:
                    return "炮塔侧面";
                case 8:
                    return "炮塔后面";
                default:
                    return "";
            }
        }

        string GetBattleType()
        {
            switch (battleType)
            {
                case 0:
                    return "标准模式";
                case 1:
                    return "复活模式";
                case 9:
                    return "天梯模式";
                default:
                    return "";
            }
        }

        public string Output()
        {
            string result = "";
            result += this.source;
            result += " -> ";
            result += this.victim;
            result += " 击中部位：";
            result += this.hitpart.ToString();
            result += " 击中类型:";
            result += this.GetDamageType();
            return result;
        }

        public Damage(string s)
        {
            if (s == "")
            {
                this.valid = false;
                return;
            }
            try
            {
                string[] mm = s.Split(',');
                source = mm[0];
                victim = mm[1];
                damage = int.Parse(mm[2]);
                type = int.Parse(mm[3]);
                victimId = int.Parse(mm[4]);
                battleType = int.Parse(mm[5]);
                battleId = int.Parse(mm[6]);
                if (int.Parse(mm[7]) == 0)
                    friend = true;
                else
                    friend = false;
                valid = true;
                if (type == 17)
                    valid = false;
                grade = int.Parse(mm[8]);
                hitpart = int.Parse(mm[9]);
            }
            catch (Exception)
            {
                valid = false;
                Log.Record("格式化Damage失败:" + s);
            }
        }

        public string parse()
        {
            string result = "";
            if (damage > 0)
            {
                result = fillspace(10, source) + "  ->  " + fillspace(10, victim);
                result = result + "    " + GetDamageType();
            }

            return result;
        }

        public string fillspace(int sum, string source)
        {
            int length = source.Length;
            for (; length < sum; length++)
            {
                source = source + " ";
            }
            if (source.Length > sum)
            {
                source = source.Substring(0, sum - 3);
                source = source + "...";
            }
            return source;
        }
    }
}
