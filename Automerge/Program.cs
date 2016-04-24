using Domain;
using MyersDiff;
using Sources.String;
using Sources.TextFile;
using System;
using System.Linq;

namespace Automerge
{
    internal class Program
    {
        private static readonly string[] m_operations = { "STRING", "FILE", "?" };

        private static void Main(string[] args)
        {
            if (args == null ||
                args.Length == 0 ||
                (args.Length != 5 && args.Length != 7) ||
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
            var outToConsole = args.Length == 5;
            ISource source = null,
                    target1 = null,
                    target2 = null;

            switch (Array.IndexOf(m_operations, operation))
            {
                case 0:
                    source = new StringSource(args[2]);
                    target1 = new StringSource(args[3]);
                    target2 = new StringSource(args[4]);
                    break;

                case 1:
                    source = new TextFileSource(args[2]);
                    target1 = new TextFileSource(args[3]);
                    target2 = new TextFileSource(args[4]);
                    break;

                default:
                    PrintUsage();
                    break;
            }
            //todo: переписать с использованием Castle.Windsor и резолвить движок по параметру engine
            var diffEngine = new MyersDiffEngine();
            var operations1 = diffEngine.GetDiff(source, target1);
            var operations2 = diffEngine.GetDiff(source, target2);

            var mergeEngine = new MergeEngine(ResolveStrategyAction.AcceptFirst, false);
            mergeEngine.ConflictOperationsResolveStrategies.Add(OperationKind.Insert, new ResolveStrategy(ResolveStrategyAction.AcceptAll, true));
            var final = mergeEngine.GetMergeOperations(operations1.ToList(), operations2.ToList());

            if (outToConsole)
            {
                Console.WriteLine("Эталон: {0}", source);
                Console.WriteLine("Первый элемент: {0}", target1);
                Console.WriteLine("Второй элемент: {0}", target2);
                Console.ReadLine();
            }
        }

        private static void PrintUsage()
        {
            Console.Write("Automerge, (c) Владимир Шудров, 2016" +
                          "{0}Утилита для обнаружения различий в двух элементах относительно эталона " +
                          "{0}и автоматическое применение изменений" +
                          "{0}с выводом результата в текстовый файл или на экран консоли." +
                          "{0}{0}Инструкция по использованию:" +
                          "{0}{0}automerge -<sourcetype> -<engine> <source> <target1> <target2>" +
                          "{0}automerge -<sourcetype> -<engine> <source> <target1> <target2> -out <filename>" +
                          "{0}{0}\t-<sourcetype> — тип источника для сравнения: file - текстовый файл, string - строка" +
                          "{0}\t-<engine> — тип diff-движка",
                          Environment.NewLine);

            Console.Write("{0}{0}\t<source> — эталон, строка или имя файла" +
                          "{0}\t<target1> — первый элемент с изменениями, строка или имя файла" +
                          "{0}\t<target2> — второй элемент с изменениями, строка или имя файла" +
                          "{0}{0}\t-out <filename> — имя файла, в который будут записаны изменения." +
                          "{0}\tЕсли параметр пропущен, то изменения будут выведены на экран консоли." +
                          "{0}{0}automerge -?" +
                          "{0}\tВывод на экран этой инструкции"
                          , Environment.NewLine);
            Console.ReadLine();
            return;
        }
    }
}