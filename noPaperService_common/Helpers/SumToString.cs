using System;

namespace noPaperService_common.Helpers
{
    public class SumToString
    {
        public string sum_to_string(long rub, byte kop)
        {
            string text = "";
            string[] array = new string[]
            {
                "один",
                "два",
                "три",
                "четыре",
                "пять",
                "шесть",
                "семь",
                "восемь",
                "девять",
                "десять",
                "одиннадцать",
                "двенадцать",
                "тринадцать",
                "четырнадцать",
                "пятнадцать",
                "шестнадцать",
                "семнадцать",
                "восемнадцать",
                "девятнадцать"
            };
            string[] array2 = new string[]
            {
                "двадцать",
                "тридцать",
                "сорок",
                "пятьдесят",
                "шестьдесят",
                "семьдесят",
                "восемьдесят",
                "девяносто"
            };
            string[] array3 = new string[]
            {
                "сто",
                "двести",
                "триста",
                "четыреста",
                "пятьсот",
                "шестьсот",
                "семьсот",
                "восемьсот",
                "девятьсот"
            };
            bool flag = kop > 0;
            text = "коп.";
            int num = 0;

            if (flag)
            {
                int num2 = kop % 10;
                int num3 = kop;
                int num4 = num3 / 10;
                switch (num)
                {
                    case 0:
                        {
                            array[0] = "одна";
                            array[1] = "две";
                            break;
                        }
                }
                bool flag3 = num3 > 0 && num3 < 20;
                if (flag3)
                {
                    text = $"{array[num3 - 1]} {text}";
                }
                else
                {
                    bool flag4 = num2 > 0;
                    if (flag4)
                    {
                        text = $"{array[num2 - 1]} {text}";
                    }
                    bool flag5 = num4 > 1;
                    if (flag5)
                    {
                        text = $"{array2[num4 - 2]} {text}";
                    }
                }
            }
            else
            {
                text = $"ноль {text}";
            }

            bool flag2 = rub < 1000000000L;
            checked
            {
                if (flag2)
                {
                    string text2 = "рублей";
                    num = 0;
                    do
                    {
                        int num2 = (int)(rub / (long)Math.Round(Math.Pow(1000.0, (double)num)) % 1000L);
                        bool flag3 = num2 > 0;
                        if (flag3)
                        {
                            int num3 = num2 % 10;
                            int num4 = num2 % 100;
                            int num5 = num2 % 1000 / 100;
                            int num6 = num4 / 10;
                            switch (num)
                            {
                                case 0:
                                    {
                                        int num7 = num3;
                                        bool flag4 = num7 == 1;
                                        if (flag4)
                                        {
                                            text2 = "рубль";
                                        }
                                        else
                                        {
                                            flag4 = (num7 >= 2 && num7 <= 4);
                                            if (flag4)
                                            {
                                                text2 = "рубля";
                                            }
                                        }
                                        bool flag5 = num4 > 9 && num4 < 20;
                                        if (flag5)
                                        {
                                            text2 = "рублей";
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        text2 = "тысяч";
                                        array[0] = "одна";
                                        array[1] = "две";
                                        int num8 = num3;
                                        bool flag6 = num8 == 1;
                                        if (flag6)
                                        {
                                            text2 = "тысяча";
                                        }
                                        else
                                        {
                                            flag6 = (num8 >= 2 && num8 <= 4);
                                            if (flag6)
                                            {
                                                text2 = "тысячи";
                                            }
                                        }
                                        bool flag7 = num4 > 9 && num4 < 20;
                                        if (flag7)
                                        {
                                            text2 = "тысяч";
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        text2 = "миллионов";
                                        array[0] = "один";
                                        array[1] = "два";
                                        int num9 = num3;
                                        bool flag8 = num9 == 1;
                                        if (flag8)
                                        {
                                            text2 = "миллион";
                                        }
                                        else
                                        {
                                            flag8 = (num9 >= 2 && num9 <= 4);
                                            if (flag8)
                                            {
                                                text2 = "миллиона";
                                            }
                                        }
                                        bool flag9 = num4 > 9 && num4 < 20;
                                        if (flag9)
                                        {
                                            text2 = "миллионов";
                                        }
                                        break;
                                    }
                            }

                            bool flag10 = num4 > 0 && num4 < 20;
                            if (flag10)
                            {
                                text = $"{array[num4 - 1]} {text2} {text}";
                            }
                            else
                            {
                                text = $"{text2} {text}";
                                bool flag11 = num3 > 0;
                                if (flag11)
                                {
                                    text = $"{array[num3 - 1]} {text}";
                                }
                                bool flag12 = num6 > 1;
                                if (flag12)
                                {
                                    text = $"{array2[num6 - 2]} {text}";
                                }
                            }

                            bool flag13 = num5 > 0;
                            if (flag13)
                            {
                                text = $"{array3[num5 - 1]} {text}";
                            }
                        }
                        else
                        {
                            bool flag14 = num == 0;
                            if (flag14)
                            {
                                text = $"ноль {text2} {text}";
                            }
                        }
                        num++;
                    }
                    while (num <= 2);
                }
                return text;
            }
        }
    }
}
