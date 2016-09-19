using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PBLib
{
    internal static class PBFunc
    {
        internal static string CorrectString(string input)
        {
            char sign;
            int posSym;
            int ExpPart = 0;
            string BufStr;
            int numZero;


            //удаление всех пробелов
            input = Regex.Replace(input, @"\s+", "");

            //изменение разделителя на запятую
            if (input.Contains(".")) input = input.Replace('.', ',');

            //запоминание знака и удаление его из строки
            if (input[0] == '+' || input[0] == '-')
            {
                sign = input[0];
                input = input.Substring(1);
            }
            else
                sign = '+';

            //работа с экспоненциальной частью
            //поиск позиции E
            input = input.ToUpper();
            posSym = input.IndexOf('E');

            //если Е найдена
            if (posSym != -1)
            {
                //выделение экспоненциальной части
                ExpPart = Convert.ToInt32(input.Substring(posSym + 1, input.Length - posSym - 1));
                //удаление экспоненциальной части
                input = input.Substring(0, posSym);
            }

            //работа с разделителем
            //поиск позиции запятой
            posSym = input.IndexOf(',');
            //коррекция строки
            //,xxxx - 0,xxxx
            if (posSym == 0) input = "0" + input;
            //xxxx - xxxx,0
            if (posSym == -1) input += ",0";
            //xxxx, - xxxx,0
            if (posSym == input.Length - 1) input += "0";
            //поик нового  значения
            posSym = input.IndexOf(',');

            //удаление незначащих нулей для дробной части
            //выделение дробной части
            BufStr = input.Substring(posSym + 1, input.Length - 1 - posSym);
            numZero = 0;
            //поиск числа незначащих нулей в конце дробной части
            for (int i = BufStr.Length - 1; i > 0; i--)
                if (BufStr[i] == '0') numZero++;
                else break;
            //удаление незначащих нулей из дробной части
            //и запись в исходное число с заменой
            input = input.Substring(0, posSym + 1) + BufStr.Substring(0, BufStr.Length - numZero);

            //удаление незначащих нулей для целой части
            BufStr = input.Substring(0, posSym);
            numZero = 0;
            //поиск числа незначащих нулей в начале целой части
            for (int i = 0; i < BufStr.Length - 1; i++)
                if (BufStr[i] == '0') numZero++;
                else break;
            //удаление незначащих нулей из целой части
            //и запись в исходное число с заменой
            input = BufStr.Substring(numZero) + input.Substring(posSym);

            posSym = input.IndexOf(',');
            if (ExpPart == 0)
                //если нет экспоненциальной части,
                //формируется выходное значение 
                input = sign + input;
            else
            {
                BufStr = Regex.Replace(input, ",", "");
                if (ExpPart < 0)
                {
                    //расчет для проверки длины целой части
                    int pos = -ExpPart - posSym;
                    //если целой части недостаточно
                    //для переноса запятой
                    if (pos >= 0) input = sign + "0," + new string('0', pos) + BufStr;
                    //если запятая переносится в целую часть
                    else input = sign + BufStr.Substring(0, -pos) + "," + BufStr.Substring(-pos);
                    //удаление незначащих нулей
                    numZero = 0;
                    for (int i = input.Length - 1; i > input.IndexOf(',') + 1; i--)
                        if (input[i] == '0') numZero++;
                        else break;
                    input = input.Substring(0, input.Length - numZero);

                    //работа с разделителем
                    //поиск позиции запятой
                    posSym = input.IndexOf(',');
                    //коррекция строки
                    //,xxxx - 0,xxxx
                    if (posSym == 0) input = "0" + input;
                    //xxxx - xxxx,0
                    if (posSym == -1) input += ",0";
                    //xxxx, - xxxx,0
                    if (posSym == input.Length - 1) input += "0";
                    //поик нового  значения
                    posSym = input.IndexOf(',');
                }
                else //ExpPart > 0
                {
                    //расчет проверки длины дробной части
                    int pos = ExpPart - (BufStr.Length - posSym);
                    //если дробной части недостаточно
                    //для переноса запятой
                    if (pos > 0) input = BufStr + new string('0', pos) + ",0";
                    //если запятая переносится в дробную часть
                    else input = BufStr.Substring(0, BufStr.Length + pos) + "," + BufStr.Substring(BufStr.Length + pos);
                    //удаление незначащих нулей
                    //и доформирование строки
                    numZero = 0;
                    for (int i = 0; i < input.IndexOf(',') - 1; i++)
                        if (input[i] == '0') numZero++;
                        else break;
                    input = sign + input.Substring(numZero);

                    //работа с разделителем
                    //поиск позиции запятой
                    posSym = input.IndexOf(',');
                    //коррекция строки
                    //,xxxx - 0,xxxx
                    if (posSym == 0) input = "0" + input;
                    //xxxx - xxxx,0
                    if (posSym == -1) input += ",0";
                    //xxxx, - xxxx,0
                    if (posSym == input.Length - 1) input += "0";
                    //поик нового  значения
                    posSym = input.IndexOf(',');
                }
            }
            return input;
        }

        internal static BinValueStorage ConvertToBin(string input, In intype)
        {
            int len = intype.MantLength + 2;

            BinValueStorage buf = new BinValueStorage();

            //знак равен первому символу входной строки
            buf.sign = (input.Substring(0, 1) == "+") ? "0" : "1";

            //удаление знака из строки
            input = input.Substring(1);

            //поиск расположения запятой
            int posSym = input.IndexOf(',');

            //преобразование целой части в двоичный вид
            string IntPart = IntPartToBin(input.Substring(0, posSym));

            //преобразование дробной части в двоичный код
            string FloatPart = FloatPartToBin(input.Substring(posSym + 1), ((BigInt)input.Substring(0, posSym)) == BigInt.Zero, len);

            //выделение знака точного преобразования дробной части
            buf.exact = (FloatPart[0] == '+') ? true : false;
            FloatPart = FloatPart.Substring(1);

            //расчет экспоненты
            if (((BigInt)input.Substring(0, posSym)) != BigInt.Zero) //целая часть != 0
            {
                buf.exp = IntExpToBin(IntPart.Length, intype);
                buf.mant = IntPart + FloatPart;
                buf.exact = PBFunc.IsStringFullZero(buf.mant.Substring(len - 2) + "0") ? true : false;
                buf.twoBits = buf.mant.Substring(len - 2 + 1, 2);
                buf.mant = buf.mant.Substring(0, len - 2);
            }
            else //целая часть равна 0
            {
                bool zero = true;
                int ZeroCount = 0;
                for (; ZeroCount < FloatPart.Length; ZeroCount++)
                    if (FloatPart[ZeroCount] != '0')
                    {
                        zero = false;
                        break;
                    }

                if (zero)
                {
                    buf.exp = IntExpToBin(0, intype);
                    buf.twoBits = "00";
                    buf.mant = new string('0', len);
                }
                else
                {
                    buf.exp = IntExpToBin(-ZeroCount, intype);
                    buf.twoBits = FloatPart.Substring(ZeroCount + len - 2, 2);
                    buf.mant = FloatPart.Substring(ZeroCount, len - 2);
                }

            }

            return buf;
        }
    }
}
