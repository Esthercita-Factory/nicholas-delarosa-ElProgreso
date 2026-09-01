using ElProgreso.Models;
using ElProgreso.Services;

namespace ElProgreso.UI;

/// <summary>
/// The six reports Don Rafael asked for, reachable from the main menu's option 12.
/// </summary>
internal static class ManagementReportsMenu
{
    public static void Run(ReportService reportService)
    {
        var back = false;
        while (!back)
        {
            Console.WriteLine(@"
Informes de gerencia:
1. ¿Cuánta plata tenemos?
2. ¿Quiénes son mis mejores asociados?
3. ¿Quiénes están dormidos?
4. ¿Cómo nos fue en un periodo?
5. ¿Cuáles fueron los movimientos más grandes?
6. ¿Quién me esta moviendo la caja?
0. Volver al menu principal");

            var option = ConsoleReader.ReadMenuOption(0, 6);
            Console.WriteLine();

            switch (option)
            {
                case 1: 
                    ShowCooperativeSummary(reportService); 
                    break;
                case 2: 
                    ShowTopMembers(reportService); 
                    break;
                case 3: 
                    ShowDormantMembers(reportService); 
                    break;
                case 4: 
                    ShowPeriodReport(reportService); 
                    break;
                case 5: 
                    ShowLargestMovements(reportService);
                    break;
                case 6:
                    ShowMemberActivity(reportService); 
                    break;
                case 0: 
                    back = true; 
                    break;
            }
        }
    }

    private static void ShowCooperativeSummary(ReportService reportService)
    {
        var summary = reportService.GetCooperativeSummary();
        
        Console.WriteLine($@"Cuanta plata tenemos:
  Saldo total de la cooperativa: {ConsoleFormat.Cop0(summary.TotalBalance)}
  Cantidad de asociados: {summary.MemberCount}
  Saldo promedio por asociado: {ConsoleFormat.Cop0(summary.AverageBalance)}");
    }

    private static void ShowTopMembers(ReportService reportService)
    {
        var topMembers = reportService.GetTopMembers();

        Console.WriteLine("Mejores asociados por saldo:");
        if (topMembers.Count == 0)
        {
            Console.WriteLine("  No hay asociados registrados.");
            return;
        }

        var position = 1;
        foreach (var member in topMembers)
            Console.WriteLine($"  {position++}. {member.DocumentNumber,-15} {member.FullName,-30} {ConsoleFormat.Cop0(member.Balance)}");
    }

    private static void ShowDormantMembers(ReportService reportService)
    {
        var dormantMembers = reportService.GetDormantMembers();

        Console.WriteLine("Asociados dormidos (sin movimientos desde que se vincularon):");
        if (dormantMembers.Count == 0)
        {
            Console.WriteLine("  Todos los asociados tienen al menos un movimiento.");
            return;
        }

        foreach (var member in dormantMembers)
            Console.WriteLine($"  {member.DocumentNumber,-15} {member.FullName,-30} vinculado el {ConsoleFormat.ShortDate(member.JoinedAt)}");
    }

    private static void ShowPeriodReport(ReportService reportService)
    {
        var from = ConsoleReader.ReadDate("Fecha inicial (dd/mm/aaaa): ");
        var to = ConsoleReader.ReadDate("Fecha final (dd/mm/aaaa): ");

        PeriodReport report;
        try
        {
            report = reportService.GetPeriodReport(from, to);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
        
        Console.WriteLine($@"Movimiento entre el {ConsoleFormat.ShortDate(report.FromDate)} y el {ConsoleFormat.ShortDate(report.ToDate)}:
  Consignado: {ConsoleFormat.Cop0(report.TotalDeposited)} ({report.DepositCount} movimiento(s))
  Retirado: {ConsoleFormat.Cop0(report.TotalWithdrawn)} ({report.WithdrawalCount} movimiento(s))
  Diferencia: {ConsoleFormat.Cop0(report.Difference)}");
    }

    private static void ShowLargestMovements(ReportService reportService)
    {
        var movements = reportService.GetLargestMovements();

        Console.WriteLine("Movimientos más grandes de la cooperativa:");
        if (movements.Count == 0)
        {
            Console.WriteLine("  Todavia no hay movimientos registrados.");
            return;
        }

        var position = 1;
        foreach (var movement in movements)
        {
            var typeLabel = movement.Type == MovementType.Deposit ? "Consignacion" : "Retiro";
            Console.WriteLine($"  {position++,2}. {ConsoleFormat.ShortDate(movement.OccurredAt)}  {typeLabel,-13} " +
                               $"{ConsoleFormat.Cop0(movement.Amount),15}  {movement.MemberFullName}");
        }
    }

    private static void ShowMemberActivity(ReportService reportService)
    {
        var activity = reportService.GetMemberActivity();

        Console.WriteLine("Quien mueve más la caja:");
        if (activity.Count == 0)
        {
            Console.WriteLine("  No hay asociados registrados.");
            return;
        }

        foreach (var entry in activity)
        {
            Console.WriteLine($"  {entry.FullName,-30} {entry.MovementCount,3} mov.  " +
                               $"consignado {ConsoleFormat.Cop0(entry.TotalDeposited)}  " +
                               $"retirado {ConsoleFormat.Cop0(entry.TotalWithdrawn)}  " +
                               $"saldo {ConsoleFormat.Cop0(entry.CurrentBalance)}");
        }
    }
}