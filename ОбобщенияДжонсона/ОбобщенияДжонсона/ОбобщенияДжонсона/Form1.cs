using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace ОбобщенияДжонсона
{
    public partial class Form1 : Form
    {
        // Массив значений, введенных в DataGridView
        int[,] array;
        int min_randon = 1;
        int max_random = 11;
        int column;
        int row;
        int[] time = new int[5];
        int[,] downtime;

        /*  Массив порядков, полученных в каждом обобщении*/
        int[,] result;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridView d = dataGridView1;

            //  Очищаем таблицу
            d.DataSource = null;
            d.Rows.Clear();
            d.Columns.Clear();

            // Создаём столбцы
            for (int i = 0; i < number_of_details.Value; i++)
                d.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
            //  Создаём строки
            d.Rows.Add(Convert.ToInt32(number_of_machines.Value));
            //  Записываем заголовки строк
            for (int i = 0; i < d.RowCount - 1; i++)
                d.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
            d.Rows[dataGridView1.RowCount - 1].HeaderCell.Value = "Суммарное время обработки";
            d.TopLeftHeaderCell.Value = "Станок|Деталь";
            //   Сделать шрифт жирным в последней строке
            for (int i = 0; i < d.ColumnCount; i++)
                d[i, d.RowCount - 1].Style.Font = new Font(this.Font, FontStyle.Bold);
        }

        //  Изменяется количество станков
        private void Number_of_machines_ValueChanged(object sender, EventArgs e)
        {
            DataGridView d = dataGridView1;

            //Добавляем строки
            if (number_of_machines.Value > d.RowCount - 1)
                d.Rows.Add(Convert.ToInt32(number_of_machines.Value - d.RowCount) + 1);
            //Удаляем строки
            if (number_of_machines.Value < d.RowCount - 1)
            {
                for (int i = dataGridView1.RowCount - 1; i > number_of_machines.Value; i--)
                    d.Rows.RemoveAt(i - 1);
            }
            //Записываем заголовки строк
            for (int i = 0; i < d.RowCount - 1; i++)
                d.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
            d.Rows[d.RowCount - 1].HeaderCell.Value = "Суммарное время обработки";
            //Сделать шрифт жирным в последней строке
            for (int i = 0; i < d.ColumnCount; i++)
                d[i, d.RowCount - 1].Style.Font = new Font(this.Font, FontStyle.Bold);
            //Сделать шрифт предпоследней стоки обычным
            for (int i = 0; i < d.RowCount - 1; i++)
                for (int j = 0; j < d.ColumnCount; j++)
                    d[j, i].Style.Font = new Font(this.Font, FontStyle.Regular);
        }

        //  Изменяется количество деталей
        private void Number_of_details_ValueChanged(object sender, EventArgs e)
        {
            DataGridView d = dataGridView1;
            //Добавляем столбцы
            if (number_of_details.Value > d.ColumnCount)
            {
                for (int i = d.ColumnCount; i < number_of_details.Value; i++)
                    d.Columns.Add(Convert.ToString(i + 1), Convert.ToString(i + 1));
            }
            //Удаляем столбцы
            if (number_of_details.Value < d.ColumnCount)
            {
                for (int i = d.ColumnCount; i > number_of_details.Value; i--)
                    d.Columns.Remove(Convert.ToString(i));
            }
            //Сделать жирной последней строчку
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                dataGridView1[i, dataGridView1.RowCount - 1].Style.Font = new Font(this.Font, FontStyle.Bold);
        }

        //Заполнение случайными числамми
        private void Random_button2_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1[j, i].Value = rand.Next(min_randon, max_random);
                }
            }

        }

        private void Action_button_Click(object sender, EventArgs e)
        {
            row = dataGridView1.RowCount;
            column = dataGridView1.ColumnCount;

            array = new int[row, column];
            Get_matrix();

            result = new int[5, column];

            //  Считаем пять обобщений
            First_generalization();
            Second_generalization();
            Third_generalization();
            Fourth_generalization();
            Fifth_generalization();

            //Отрисовка
            Drow();

            //Запись в labels
            Label[] labels = new Label[] { first, second, third, fourth, fifth };

            for(int i = 0; i < labels.Length; i++)
            {
                labels[i].Text = "";
                for (int j = 0; j < column; j++)
                {
                    labels[i].Text += result[i, j] + " ";
                }
                labels[i].Text += " Время обработки = " + time[i];
            }
        }

        //  Записать матрицу из DataGridView в глобальный массив array
        private void Get_matrix()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    array[i, j] = Convert.ToInt32(dataGridView1[j, i].Value);
                }
            }
        }

        //  При изменении значений в клетках пересчитать суммарное время обработки детали
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Calculate_total_processing_time();
        }

        //  Вычислить суммарное время обработки внизу таблицы
        private void Calculate_total_processing_time()
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                int sum = 0;
                for (int j = 0; j < dataGridView1.RowCount - 1; j++)
                {
                    sum += Convert.ToInt32(dataGridView1[i, j].Value);
                }
                dataGridView1[i, dataGridView1.RowCount - 1].Value = sum;
            }
        }

        /*  Первое обобщение
         *  
         *  В обработку запускаются детали,
         *  требующие минимума времени обработки на первом станке 
         *  в порядке возрастания этого времени */
        private void First_generalization()
        {
            //  Массив из порядкового номера и времени обработки детали на первом станке
            int[,] arr = new int[2, column];

            for (int j = 0; j < column; j++)
            {
                arr[0, j] = j; //   порядковый номер детали
                arr[1, j] = array[0, j]; // время обработки на первом станке
            }

            //  Сортируем по времени обработки в порядке возрастания
            Sort(arr, true);

            //  Возвращение результата в глобальную переменную
            for (int j = 0; j < column; j++)
                result[0,j] = arr[0, j] + 1;
        }

        //  Перестановка для сортировки
        private void Swap(int[,] arr, int left, int right)
        {
            int[] temp = new int[2];
            if (left != right)
            {
                for (int i = 0; i < 2; i++)
                {
                    temp[i] = arr[i, left];
                    arr[i, left] = arr[i, right];
                    arr[i, right] = temp[i];
                }
            }
        }

        //  Сортировка пузырьком
        private void Sort(int[,] arr, bool grow)
        {
            bool swapped;

            do
            {
                swapped = false;
                for (int i = 1; i < arr.GetLength(1); i++)
                {
                    if (grow == true)
                    {
                        if (arr[1, i - 1].CompareTo(arr[1, i]) > 0)
                        {
                            Swap(arr, i - 1, i);
                            swapped = true;
                        }
                    }
                    else
                    {
                        if (arr[1, i - 1].CompareTo(arr[1, i]) < 0)
                        {
                            Swap(arr, i - 1, i);
                            swapped = true;
                        }
                    }

                }
            } while (swapped != false);
        }

        /*  Второе обобщение
         * 
         *  В обработку запускаются детали 
         *  с максимальным временем обработки на последнем станке
         *  в порядке убывания этого времени */
        private void Second_generalization()
        {
            //  Массив из порядкового номера и времени обработки детали на последнем станке
            int[,] arr = new int[2, column];

            for (int j = 0; j < column; j++)
            {
                arr[0, j] = j; //   порядковый номер детали
                arr[1, j] = array[row - 2, j]; //   время обработки на последнем станке
            }

            //  отсортировать в порядке убывания
            Sort(arr, false);


            //  Возвращение результата в глобальную переменную
            for (int j = 0; j < column; j++)
                result[1, j] = arr[0, j] + 1;
        }

        /*  Третье обобщение
         * 
         *  В обработку запускаются детали, 
         *  у которых «узкое место» находится дальше от начала процесса обработки. 
         *  («узкое место» для детали – есть станок, на котором обработка детали 
         *  занимает наибольшее время (максимальное) */
        private void Third_generalization()
        {
            //  Двумерный массив - копия array
            int[,] arr = new int[row, column];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    arr[i, j] = array[i, j];
                }
            }

            //  максимальное значение времени обработки детали
            int max_value = 0;
            //  на каком станке
            int max_value_machine = -1;

            //  ищем максимальное время обработки и на каком станке обнаружено
            for (int k = 0; k < column; k++)
            {
                for (int i = 0; i < row - 1; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        if (arr[i, j] >= max_value)
                        {
                            max_value = arr[i, j];
                            max_value_machine = j;
                        }
                    }
                }
                //  записываем в результат
                result[2, k] = max_value_machine + 1;
                //  зануляем для новой итерации
                max_value = 0;

                //  для выбранного станка зануляем значения (оставляем минус один станок для дальнейшей обработки)
                for (int i = 0; i < row; i++)
                    arr[i, max_value_machine] = 0;
            }
        }

        /*  Четвертое обобщение
         
            Вначале обрабатываются детали, 
            у которых суммарное время обработки на всех станках 
            максимальное в порядке убывания этого времени. */
        private void Fourth_generalization()
        {
            //  Массив из порядкового номера и времени суммарной обработки детали
            int[,] arr = new int[2, column];

            for (int j = 0; j < column; j++)
            {
                arr[0, j] = j;
                arr[1, j] = array[row - 1, j];
            }

            //  Сортируем по убыванию суммарного времени обработки
            Sort(arr, false);

            //  Возвращение результата
            for (int j = 0; j < column; j++)
                result[3, j] = arr[0, j] + 1;
        }

        /*  Пятое обобщение
         * 
         * Для каждой детали ищется сумма мест во всех полученных решений. 
         * Располагаются детали в порядке возрастания суммы мест*/
        private void Fifth_generalization()
        {
            //  Массив из порядкового номера детали и суммы мест
            int[,] arr = new int[2, column];

            for (int j = 0; j < column; j++)
            {
                arr[0, j] = j;
                arr[1, j] = 0; //   пока сумма равна нулю
            }

            //  Складываем места для каждой детали

            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    arr[1, result[i, j] - 1] += j + 1;
                }
            }


            //  Сортируем по возрастанию
            Sort(arr, true);

            //  Возвращаем результат
            for (int j = 0; j < column; j++)
                result[4, j] = arr[0, j] + 1;
        }

        //Рисовка
        private void Drow()
        {
            //пять двумерных массивов значений из DataGridView. По одному для каждого обобщения
            int[][,] arr = new int[5][,]
            {
                new int [row - 1, column],
                new int [row - 1, column],
                new int [row - 1, column],
                new int [row - 1, column],
                new int [row - 1, column]
            };

            for (int i = 0; i < row - 1; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    for (int z = 0; z < 5; z++)
                    {
                        arr[z][i, j] = array[i, j];
                    }

                }
            }

            //Пять двумерных массивов очередей. По одному для каждого обобщения
            int[][,] queue = new int[5][,]
            {
                new int [row - 1, column],
                new int [row - 1, column],
                new int [row - 1, column],
                new int [row - 1, column],
                new int [row - 1, column]
            };

            for (int j = 0; j < column; j++)
            {
                for (int z = 0; z < 5; z++)
                {
                    queue[z][0, j] = result[z, j];
                }
            }

            //Массив DataGridView для вывода
            DataGridView[] dataGridView_array = new DataGridView[] { dataGridView2, dataGridView3, dataGridView4, dataGridView5, dataGridView6 };

            //Получаем цвета для деталей
            Color[] COLOR = GetColor();

            downtime = new int[row - 1, 5];

            //По очереди отрисовываем график
            for (int i = 0; i < 5; i++)
            {
                time[i] = Drow_graphic(arr[i], queue[i], dataGridView_array[i], COLOR, i);
            }
        }

        //  Цвета для деталей на графике
        private Color[] GetColor()
        {
            Random rnd = new Random(); // Создаём массив цветов для каждой столбца 
            Color[] COLOR = new Color[dataGridView1.ColumnCount];
            Color c, repeat;
            repeat = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            c = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            for (int i = 0; i < COLOR.Length; i++)
            {
                while (c == repeat)
                    c = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                repeat = c;
                COLOR[i] = c;
            }
            return COLOR;
        }

        int[] times_goodbye;
        //Рисуем один график
        private int Drow_graphic(int[,] arr, int[,] queue, DataGridView d, Color[] COLOR, int generalization)
        {
            times_goodbye = new int[row - 1];
            d.TopLeftHeaderCell.Value = "Станок";

            // Очищаем таблицу
            if (d.RowCount > 9)
                for (int i = d.RowCount - 1; i >= 0; i--)
                    d.Rows.RemoveAt(i);
            if (d.ColumnCount > 9)
            {
                for (int i = d.ColumnCount; i > 0; i--)
                    d.Columns.Remove(Convert.ToString(i));
            }

            int k = 0;
            int[] downtime_now = new int[row - 1];
            while (true)
            {
                //  Проверка - если ни разу не был закрашен квадрат в столбце, то детали кончились.
                bool blank = true;

                // Добавляем столбец
                d.Columns.Add(Convert.ToString(k + 1), Convert.ToString( k + 1));
                d.Columns[k].Width = 20;

                //  Таблица была пуста, нужно добавить строки
                if (k == 0)
                {
                    //  Добавляем строки
                    d.Rows.Add(row - 1);
                    //  Записываем заголовки строк
                    for (int i = 0; i < d.RowCount; i++)
                    {
                        d.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
                    }
                }

                //  i -  станок
                //  queue[i, 0] - номер детали в очереди (от 1 до N)
                for (int i = row - 2; i >= 0; i--)
                {
                    if (queue[i, 0] == 0)
                    {
                        ;// ничего не делать, очереди на данном станке нет.
                        //Увеличим время простоя
                        downtime_now[i]++;
                    }
                    else
                    {
                        blank = false;
                        //  Закрасим
                        d[k, i].Value = queue[i, 0];
                        d[k, i].Style.BackColor = COLOR[queue[i, 0] - 1];
                        // Уменьшим оставшееся время обработки на 1
                        Minus(arr, i, queue[i, 0] - 1, queue);

                        //Если очередь покинули все детали, то время простоя передаем в глобальную переменную
                        if (times_goodbye[i] == column)
                            downtime[i, generalization] = downtime_now[i];
                    }
                }
                if (blank == true)
                    break;
                k++;
            }
            //  k - время обработки последней детали
            return k;
        }

        /*  Уменьшить оставшееся время обработки на единицу*/
        private void Minus(int[,] arr, int machine, int detail, int[,] queue)
        {
            arr[machine, detail]--;
            //   Если деталь закончила обрабатываться на этом станке, то перенести в следующую очередь
            if (arr[machine, detail] == 0)
            {
                Dequeue(queue, machine);
            }
        }

        //  Удалить деталь с начала предыдущей очереди, поставить в конец следующей - line - станок
        private void Dequeue(int[,] arr,int line)
        {
            times_goodbye[line]++;

            int temp = arr[line, 0];
            arr[line, 0] = 0;

            //переносим детали к началу очереди
            //пока не будет достигнут конец очереди, или пока не закончится массив
            for(int j = 1; (j < column) && (arr[line,j] != 0) ; j++)
            {
                arr[line, j - 1] = arr[line, j];
                arr[line, j] = 0;
            }
            //если это был последний станок
            if (line >= row - 2)
                return;
            else
            {
                for(int j = 0; j < column; j++)
                {
                    if (arr[line + 1, j] == 0)
                    {
                        //перенесли на следующий станок в конец очереди
                        arr[line + 1, j] = temp;
                        break;
                    }
                }
            }
        }

        private void Save_button_Click(object sender, EventArgs e)
        {
            if (array == null)
                return;
            string p_path = "", path = "";
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileInfo fi = new FileInfo(ofd.SelectedPath);
                DateTime now = DateTime.Now;
                p_path = ofd.SelectedPath /*fi.DirectoryName*/ + "\\" + now.ToString("d");
            }
            else
                return;

            path = p_path + ".txt";
            for ( int i = 1; File.Exists(path); i++ )
            {
                path = p_path + "_" + i + ".txt";
            }

            MessageBox.Show(path);

            StreamWriter writer = new StreamWriter(path);

            for(int i = 0; i < 5; i++)
            {
                writer.WriteLine(i + 1 + " обобщение");
                writer.WriteLine("***");
                writer.WriteLine("Очередь");
                for ( int j = 0; j < column; j++)
                {
                    writer.Write(result[i, j] + " ");
                }
                writer.WriteLine();
                writer.WriteLine("***");
                writer.WriteLine("Время обработки - " + time[i] + ". ");
                writer.WriteLine("***");
                writer.WriteLine("Время простоя каждого станка");
                for(int j = 0; j < row - 1; j++)
                {
                    writer.Write(downtime[j, i] + " ");
                }
                writer.WriteLine("");
                writer.WriteLine("");
            }

            writer.Close();
        }
    }
}
