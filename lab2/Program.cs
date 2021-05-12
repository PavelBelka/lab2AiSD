using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace lab2
{
    class Program
    {
        public static double[] array_x;
        public static double[] array_y;
        public static List<int> point;
        private static int amount_of_points = 0;
        private static List<int> tops;
        public static String[] data_x = null;
        public static String[] data_y = null;
        static void Main(string[] args){
            Stopwatch stopwatch = new Stopwatch();
            //read_file();
            Console.WriteLine("Введите количество точек: ");
            amount_of_points = Convert.ToInt32(Console.ReadLine());
            array_x = new double[amount_of_points];
            array_y = new double[amount_of_points];
            point = new List<int>();
            for (int i = 0; i < amount_of_points; i++)
            {
                point.Add(i);
            }
            Console.WriteLine("Генерация координат точек...");
            Random_coordinate(amount_of_points);
            //for (int i = 0; i < amount_of_points; i++)
            //{
            //    array_x[i] = Convert.ToInt32(data_x[i]);
            //    array_y[i] = Convert.ToInt32(data_y[i]);
            //}
            Console.WriteLine("Координаты точек сгенерированы.\nВычисление оболочки...");
            stopwatch.Start();
            AlgorithmDjarviz(amount_of_points);
            //QuickHull(amount_of_points);
            stopwatch.Stop();
            Console.WriteLine("Время выполнения алгоритма: " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Создание файла...");
            Save_file(amount_of_points);
            Console.WriteLine("Запуск окна с графиком.");
            ProcessStartInfo startInfo = new ProcessStartInfo("python");
            Process process = new Process();
            startInfo.Arguments = "main.py";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            process.StartInfo = startInfo;
            process.Start();
            Console.ReadKey();
        }

        public static void Random_coordinate(int N){
            Random rnd = new Random();
            for(int i = 0; i < N; i++){
                array_x[i] = -10 + rnd.NextDouble() * 20;
                array_y[i] = -10 + rnd.NextDouble() * 20;
            }
        }

        public static void Save_file(int N){
            string Patch = "data.txt";
            string x_coordinate = null;
            string y_coordinate = null;
            StreamWriter sw = null;

            try{
                sw = new StreamWriter(Patch, false, System.Text.Encoding.Default);
                Console.WriteLine("Файл создан. Запись данных...");
                for(int i = 0; i < N; i++){
                    x_coordinate += Convert.ToString(array_x[i]) + " ";
                    y_coordinate += Convert.ToString(array_y[i]) + " ";
                }
                sw.WriteLine(x_coordinate);
                sw.WriteLine(y_coordinate);
                x_coordinate = null;
                y_coordinate = null;
                for(int i = 0; i < tops.Count; i++){
                    x_coordinate += Convert.ToString(array_x[tops[i]]) + " ";
                    y_coordinate += Convert.ToString(array_y[tops[i]]) + " ";
                }
                sw.WriteLine(x_coordinate);
                sw.WriteLine(y_coordinate);
            }
            catch (Exception e){
                Console.WriteLine("Ошибка в файле:" + e.Message);
            }
            finally{
                if (sw != null){
                    sw.Close();
                    Console.WriteLine("Данные записаны.");
                }
            }
        }

        public static void read_file(){
            string Patch = "data.txt";
            StreamReader sr = null;
            try{
                sr = new StreamReader(Patch);
                data_x = sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                data_y = sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                amount_of_points = 60;
            }
            catch (Exception e){
                Console.WriteLine("Ошибка в файле:" + e.Message);
            }
            finally{
                if (sr != null){
                    sr.Close();
                    Console.WriteLine("Данные записаны.");
                }
            }
        }

        public static double Rotate(double A_x, double A_y, double B_x, double B_y, double C_x, double C_y){
            double Orientation = (B_x - A_x) * (C_y - A_y) - (C_x - A_x) * (B_y - A_y);
                return Orientation;
        }

        public static void AlgorithmDjarviz(int N)
        {
            int end_point, buf;
            tops = new List<int>();
            //Находим точку с наибольшей координатой x и меньшей координатой y
            for (int i = 1; i < N; i++)
                if (array_x[point[i]] < array_x[point[0]] || (array_x[point[i]] == array_x[point[0]] && (array_y[point[i]] < array_y[point[0]]))){
                    buf = point[0];
                    point[0] = point[i];
                    point[i] = buf;
                }
            tops.Add(point[0]);
            point.RemoveAt(0);
            point.Add(tops[0]);
            while(true){
                end_point = 0;
                for (int i = 1; i < point.Count(); i++){
                    if(Rotate(array_x[tops[tops.Count - 1]], array_y[tops[tops.Count - 1]], array_x[point[end_point]], array_y[point[end_point]], array_x[point[i]], array_y[point[i]]) < 0){
                        end_point = i;
                    }
                }
                if (point[end_point] == tops[0])
                    break;
                else{
                    tops.Add(point[end_point]);
                    point.RemoveAt(end_point);
                }
            }        
        }

        public static void QuickHull(int N)
        {
            int left_point = 0, right_point = 0;
            double Degree = 0;
            List<int> S1 = new List<int>();
            List<int> S2 = new List<int>();
            tops = new List<int>();
            //находим самую левую и самую правую точку
            for (int i = 1; i < N; i++){
                if(array_x[point[i]] < array_x[left_point] || (array_x[point[i]] == array_x[left_point] && (array_y[point[i]] < array_y[left_point])))
                    left_point = point[i];
                else if(array_x[point[i]] > array_x[right_point] || (array_x[point[i]] == array_x[right_point] && (array_y[point[i]] > array_y[right_point])))
                    right_point = point[i];
            }
            for (int i = 0; i < N; i++){
                if (point[i] != left_point && point[i] != right_point){
                    Degree = Rotate(array_x[left_point], array_y[left_point], array_x[right_point], array_y[right_point], array_x[point[i]], array_y[point[i]]);
                    if (Degree >= 0)
                        S1.Add(point[i]);
                    else
                        S2.Add(point[i]);
                }
            }
            tops.Add(left_point);
            QuickHull(S1, left_point, right_point, 1);
            tops.Add(right_point);
            QuickHull(S2, right_point, left_point, 1);
        }

        public static void QuickHull(List<int> s, int left, int right, int inv){
            double Square_max = 0, Square = 0;
            List<int> sub1 = new List<int>();
            List<int> sub2 = new List<int>();
            int index = -1, Degree = 0, Rot = 0;
            for (int i = 0; i < s.Count(); i++){
                Square = (Rotate(array_x[left], array_y[left], array_x[right], array_y[right], array_x[s[i]], array_y[s[i]]) / 2.0);
                if (Square > 0)
                    Rot = 1;
                else if (Square < 0)
                    Rot = -1;
                else 
                    Rot = 0;
                if (Square * inv > Square_max && (Rot == inv || Rot == 0)){
                    Square_max = Square * inv;
                    index = s[i];
                }
            }
            if (index >= 0)
            {
                for (int i = 0; i < s.Count(); i++){
                    if (s[i] != left && s[i] != right){
                        if (Rotate(array_x[left], array_y[left], array_x[index], array_y[index], array_x[s[i]], array_y[s[i]]) >= 0)
                            sub1.Add(s[i]);
                        else
                            sub2.Add(s[i]);
                    }
                }
                QuickHull(sub1, left, index, inv);
                tops.Add(index);
                QuickHull(sub2, index, right, inv);
            }
            else
                return;
        }
    }
}
