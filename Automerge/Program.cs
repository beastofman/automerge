using Domain;
using MyersDiff;
using Sources.String;
using Sources.TextFile;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Automerge
{
    internal class Program
    {
        /*
         * -string "abc" "abe" "abd"
         * Параметры командной строки для вывода изменения в двух строках на экран
         */

        /*
         * -string "abc" "abe" "abd" "out.txt"
         * Параметры командной строки для вывода изменения в двух строках в файл out.txt
         */
        /*
         * -file "Source.txt" "Target1.txt" "Target2.txt" "out.txt"
         * Параметры командной строки для вывода изменения в двух текстовых файлах в файл out.txt
         */
        /*
         * -file "Source.txt" "Target1.txt" "Target2.txt"
         * Параметры командной строки для вывода изменения в двух текстовых файлах на экран
         */

        /// <summary>
        /// Список доступных операций
        /// </summary>
        private static readonly string[] m_operations = { "STRING", "FILE", "?" };

        private static void Main(string[] args)
        {
            if (args == null ||
                args.Length == 0 ||
                (args.Length != 4 && args.Length != 5) ||
                args.Any(string.IsNullOrEmpty))
            {
                PrintUsage();
                return;
            }

            var operation = args[0]?.Replace("-", string.Empty).ToUpperInvariant();
            if (string.IsNullOrEmpty(operation))
            {
                PrintUsage();
                return;
            }
            var outToConsole = args.Length == 4;
            ISource source = null,
                    target1 = null,
                    target2 = null;

            switch (Array.IndexOf(m_operations, operation))
            {
                case 0:
                    source = new StringSource(args[1]);
                    target1 = new StringSource(args[2]);
                    target2 = new StringSource(args[3]);
                    break;

                case 1:
                    try
                    {
                        source = new TextFileSource(args[1]);
                        target1 = new TextFileSource(args[2]);
                        target2 = new TextFileSource(args[3]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Во время загрузки файлов произошла ошибка:");
                        Console.WriteLine(ex.Message);
                        PrintUsage();
                        return;
                    }

                    break;

                default:
                    PrintUsage();
                    break;
            }
            var diffEngine = new MyersDiffEngine();
            var operations1 = diffEngine.GetDiff(source, target1);
            var operations2 = diffEngine.GetDiff(source, target2);

            var mergeEngine = new MergeEngine(ResolveStrategyAction.AcceptFirst, false);
            mergeEngine.ConflictOperationsResolveStrategies.Add(OperationKind.Insert, new ResolveStrategy(ResolveStrategyAction.AcceptAll, true));
            var final = mergeEngine.GetMergeOperations(operations1.ToList(), operations2.ToList());
            Console.WriteLine("Эталон: {0}", source);
            Console.WriteLine("Первый элемент: {0}", target1);
            Console.WriteLine("Второй элемент: {0}", target2);
            var sb = new StringBuilder();
            foreach (var op in final)
            {
                switch (op.Kind)
                {
                    case OperationKind.Equal:
                    case OperationKind.Insert:
                    default:

                        sb.AppendFormat("{0}{1}", op.IsConflict ? " Конфликт! >" : string.Empty, op.Source[op.Index]);
                        break;

                    case OperationKind.Unknown:
                    case OperationKind.Delete:
                        break;
                }
            }
            Console.WriteLine();
            Console.WriteLine("---РЕЗУЛЬТАТ---");
            if (outToConsole)
            {
                Console.WriteLine(sb.ToString());
            }
            else
            {
                try
                {
                    using (var wrtr = new StreamWriter(args[4]))
                    {
                        wrtr.Write(sb.ToString());
                        wrtr.Close();
                    }
                    Console.WriteLine("Изменения записаны в файл {0}", args[4]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Во время записи результата в файл произошла ошибка:");
                    Console.WriteLine(ex.Message);
                    PrintUsage();
                    return;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Нажмите <Enter> для завершения работы...");
            Console.ReadLine();
        }

        private static void PrintUsage()
        {
            Console.Write("Automerge, (c) Владимир Шудров, 2016" +
                          "{0}Утилита для обнаружения различий в двух элементах относительно эталона " +
                          "{0}и автоматическое применение изменений" +
                          "{0}с выводом результата в текстовый файл или на экран консоли." +
                          "{0}{0}Инструкция по использованию:" +
                          "{0}{0}automerge -<sourcetype> <source> <target1> <target2>" +
                          "{0}automerge -<sourcetype> <source> <target1> <target2> <outfilename>" +
                          "{0}{0}\t-<sourcetype> — тип источника для сравнения: file - текстовый файл, string - строка" +
                          "{0}{0}\t<source> — эталон, строка или имя файла" +
                          "{0}\t<target1> — первый элемент с изменениями, строка или имя файла" +
                          "{0}\t<target2> — второй элемент с изменениями, строка или имя файла" +
                          "{0}{0}\t<outfilename> — имя файла, в который будут записаны изменения." +
                          "{0}\tЕсли параметр пропущен, то изменения будут выведены на экран консоли." +
                          "{0}{0}automerge -?" +
                          "{0}\tВывод на экран этой инструкции"
                          , Environment.NewLine);
            Console.ReadLine();
        }
    }
}