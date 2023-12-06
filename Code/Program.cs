namespace Code
{
    public class Program
    {
        static void Main()
        {
            //Считываем с клавы исходные данные
            Console.WriteLine("Введите коэфциенты в одну строку\nРазделять значения ПРОБЕЛОМ\nВ числе разделитель между целой и дробной частью - запятая");
            Console.Write("Коэфициенты: ");
            List<Node> InputArray = GetCoefFromConsole();

            //считываем число - сколько можно суммировать за раз
            Console.Write("Сколько нужно суммировать за раз? ");
            int CoutnMaxSum = Convert.ToInt32(Console.ReadLine());

            //Получение массива данных
            List<List<Node>> FullArray = GetFullArray(CoutnMaxSum, InputArray);      
                        
            //Вывод всего массива данных
            //Console.WriteLine("-------Массив---------");
            //for (int i = 0; i < FullArray.Count; i++)
            //    Print(FullArray[i]);

            //получение значения функций
            List<string> strings = GetValueFunc(FullArray);

            //вывод значений функции
            Console.WriteLine("--------Значения функций----------");
            string alphabet = "abcdefghijklmnopqrstuvwxyz";//буквы для вывода
            char[] chars = alphabet.ToCharArray();
            for (int i = 0; i < strings.Count; i++)
                Console.WriteLine($"A({chars[i]}) = {strings[i]}");

            //подсчёт и вывод L среднего
            double L_middle = 0;
            for (int i = 0; i < strings.Count; i++)
                L_middle += strings[i].Length * FullArray[0][i].Value;
            L_middle = Math.Round(L_middle, 2);
            Console.WriteLine($"Ответ Lсреднее: {L_middle}");
            Console.ReadKey();
        }

        /// <summary>
        /// Получение всех столбцов
        /// </summary>
        /// <returns>Лист с листами из узлов</returns>
        static List<List<Node>> GetFullArray(int CountMaxSum, List<Node> InputArray)
        {
            //кол-во суммирований при формировании первого столбца
            List<List<Node>> FullArray = [InputArray];
            int FirstIterationCountMaxSum = ((InputArray.Count - 1) % (CountMaxSum - 1)) + 1;

            //Первая итерация
            if (FirstIterationCountMaxSum > 1)
                GetColumn(FullArray, FirstIterationCountMaxSum, 1);
            //Остальные итерации
            for (int i = FirstIterationCountMaxSum > 1 ? 2 : 1; FullArray[i-1].Count>1; i++)
                GetColumn(FullArray, CountMaxSum, i);
            return FullArray;
        }

        /// <summary>
        /// Формирование столбца узлов
        /// </summary>
        /// <remarks>
        /// <para>
        /// Входные параметры:
        /// </para>
        /// <list type="table">
        /// <item>
        /// <term>FullArray</term>
        /// <description>
        /// Лист из листов, где каждый вложенный лист представляет из себя столбец из узлов
        /// </description>
        /// </item>
        /// <item>
        /// <term>CountSum</term>
        /// <description>
        /// Кол-во суммирований узлов за одну итерацию
        /// </description>
        /// </item>
        /// <item>
        /// <term>i</term>
        /// <description>
        /// Номер формируемого столбца с узлами
        /// </description>
        /// </item>
        /// <item>
        /// </item>
        /// </list>
        /// </remarks>
        static void GetColumn(List<List<Node>> FullArray, int CountSum, int i)
        {
            //выделение памяти под новый массив
            FullArray.Add([]);

            //если элементов не меньше, чем CountSum, то находим сумму наименьших CountSum узлов
            if (FullArray[i - 1].Count >= CountSum)
            {
                //получаем последний сформированный столбец
                List<Node> LastListNode = FullArray[i - 1];

                //берём последние маленькие элементы в кол-ве CountSum
                List<Node> Nodes = new();
                for (int j = 0; j < CountSum; j++)
                    Nodes.Add(LastListNode[LastListNode.Count - 1 - j]);

                //получаем значения узлов
                List<double> NodesValue = new();
                for (int j = 0; j < CountSum; j++)
                    NodesValue.Add(Nodes[j].Value);

                //получаем сумму всех значений
                double SumValueNode = 0;
                for (int j = 0; j < CountSum; j++)
                    SumValueNode += Math.Round(NodesValue[j], 2);

                //копируем в новый лист всё из предыдущего, кроме суммируемых элементов
                for (int j = 0; j < FullArray[i - 1].Count - CountSum; j++)
                {
                    //формируем новый узел с тем же значением
                    Node CopyNode = new(FullArray[i - 1][j].Value);
                    //добавляем в массив
                    FullArray[i].Add(CopyNode);
                    //старому узлу делаем ссылку на его родителя
                    FullArray[i - 1][j].NextNode = CopyNode;
                }

                //формируем новый узел из суммы CountSum чисел
                Node NewNode = new(SumValueNode);

                //добавляем новый узел
                FullArray[i].Add(NewNode);

                //на узлы, который были были сложены, накладываем ссылку на их нового потомка и ставим новые значения 0 и 1
                int index = CountSum;
                index--;
                foreach (Node node in Nodes)
                {
                    node.NextNode = NewNode;
                    node.Index = $"{index--}";
                }

                //сортируем по убыванию
                Sort(FullArray[i]);
            }
        }

        /// <summary>
        /// Получение массива строк с результатами функции
        /// </summary>
        /// <param name="FullArray">Все узлы</param>
        /// <returns>Лист строк</returns>
        static List<string> GetValueFunc(List<List<Node>> FullArray)
        {
            List<string> str = [];
            //первый столбец
            List<Node> Nodes = FullArray[0];
            //Для каждой строки находим значение функции
            for (int i = 0; i < Nodes.Count; i++)
            {
                //значение функции для строки
                string Answer = "";
                //получаем значение из первого столбца
                Node? node = Nodes[i];
                //пока есть связь, берём индекс
                while (node != null)
                {
                    Answer += node.Index;
                    node = node.NextNode;
                }
                //переворачиваем строку ответа
                char[] charsAnswer = Answer.ToCharArray();
                Array.Reverse(charsAnswer);
                Answer = string.Join("", charsAnswer);
                //добавляем в массив результатов
                str.Add(Answer);
            }
            return str;
        }

        /// <summary>
        /// Вывод списка узлов
        /// </summary>
        /// <param name="Nodes"></param>
        static void Print(List<Node> Nodes)
        {  
            string Output = "";
            for (int i = 0; i < Nodes.Count; i++)
            {
                Output += Nodes[i].Value;
                Output += "->";
                Output += Nodes[i]?.NextNode?.Value;
                if (Nodes[i].Index != "")
                    Output += $"{{{Nodes[i].Index}}}  ";
                else
                    Output += "  ";
            }
            Console.WriteLine(Output);
        }

        /// <summary>
        /// Получение коэффицинтов с консоли
        /// </summary>
        /// <returns>Список из узлов</returns>
        static List<Node> GetCoefFromConsole()
        {
            string? input = Console.ReadLine();
            string[]? inputArray = input?.Split();
            List<Node> result = [];
            for (int i = 0; i < inputArray?.Length; i++)
                result.Add(new Node(Math.Round(Convert.ToDouble(inputArray[i]), 2)));
            return result;
        }

        /// <summary>
        /// Сортировка узлов в списке
        /// </summary>
        static void Sort(List<Node> Nodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
                for (int j = i + 1; j < Nodes.Count; j++)
                    if (Nodes[i].Value < Nodes[j].Value)
                        (Nodes[i], Nodes[j]) = (Nodes[j], Nodes[i]);
        }
    }

    /// <summary>
    /// Класс, содержащий в себе информацию об коэффициенте 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Свойства:
    /// </para>
    /// <list type="table">
    /// <item>
    /// <term>Index: string </term>
    /// <description>
    /// Содержит в себе положительное число, если этот узел перешёл в новый при суммировании узлов.
    /// Иначе имеет пустое значение.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Value: double</term>
    /// <description>
    /// Значение узла
    /// </description>
    /// </item>
    /// <item>
    /// <term>NextNode: Node</term>
    /// <description>
    /// Ссылка на следующий узел
    /// </description>
    /// </item>
    /// <item>
    /// </item>
    /// </list>
    /// </remarks>
    public class Node
    {
        public string Index { get; set; } = "";
        public double Value { get; set; }
        public Node? NextNode { get; set; } = null;
        public Node(double Value)
        {
            this.Value = Value;
        }
    }
}
//0,41 0,12 0,09 0,08 0,07 0,05 0,03 0,03 0,03 0,03 0,03 0,02 0,01 
