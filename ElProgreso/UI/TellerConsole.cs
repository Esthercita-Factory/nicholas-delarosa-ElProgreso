using ElProgreso.Models;
using ElProgreso.Services;

namespace ElProgreso.UI;

/// <summary>
/// The teller-facing menu loop.
/// Every user-visible string here is in Spanish on purpose: this is the only layer the person at the counter ever sees.
/// </summary>
/// <param name="memberService"></param>
/// <param name="movementService"></param>
/// <param name="reportService"></param>
internal sealed class TellerConsole(MemberService memberService, MovementService movementService, ReportService reportService)
{
    public async Task RunAsync()
    {
        Console.WriteLine(@"Cooperativa Financiera El Progreso
Sistema de ventanilla para asociados");

        var running = true;
        while (running)
        {
            Console.WriteLine();
            PrintMainMenu();
            var option = ConsoleReader.ReadMenuOption(0, 12);
            Console.WriteLine();

            switch (option)
            {
                case 1: 
                    RegisterMember(); 
                    break;
                case 2: 
                    ListMembers(); 
                    break;
                case 3: 
                    FindByDocument(); 
                    break;
                case 4: 
                    FindByName(); 
                    break;
                case 5: 
                    UpdateMember(); 
                    break;
                case 6: 
                    DeleteMember(); 
                    break;
                case 7: 
                    ShowBalance(); 
                    break;
                case 8: 
                    await ShowBalanceInUsdAsync(); 
                    break;
                case 9: 
                    RegisterMovement(MovementType.Deposit); 
                    break;
                case 10: 
                    RegisterMovement(MovementType.Withdrawal); 
                    break;
                case 11: 
                    ShowMovements(); 
                    break;
                case 12: 
                    ManagementReportsMenu.Run(reportService); 
                    break;
                case 0: 
                    running = false; 
                    break;
            }
        }

        Console.WriteLine("\nHasta pronto.");
    }

    private static void PrintMainMenu()
    {
        Console.WriteLine(@"¿Qué necesita hacer?
 1. Registrar un nuevo asociado
 2. Listar todos los asociados
 3. Buscar un asociado por documento
 4. Buscar un asociado por nombre
 5. Actualizar los datos de un asociado
 6. Eliminar un asociado
 7. Consultar saldo
 8. Consultar saldo en dólares (TRM)
 9. Registrar consignación
10. Registrar retiro
11. Ver los movimientos de un asociado
12. Consultar informes de gerencia
 0. Salir");
    }

    private void RegisterMember()
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento: ");
        var fullName = ConsoleReader.ReadRequiredText("Nombre completo: ");
        var phoneNumber = ConsoleReader.ReadOptionalText("Teléfono (Enter para omitir): ");
        var address = ConsoleReader.ReadOptionalText("Dirección (Enter para omitir): ");

        try
        {
            var member = memberService.Register(documentNumber, fullName, phoneNumber, address);
            Console.WriteLine($"Asociado registrado. {member.FullName} queda con la cuenta en {ConsoleFormat.Cop0(0)}.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"No se pudo registrar: {ex.Message}");
        }
    }

    private void ListMembers()
    {
        var members = memberService.GetAll();
        if (members.Count == 0)
        {
            Console.WriteLine("Todavía no hay asociados registrados.");
            return;
        }

        Console.WriteLine($"{members.Count} asociado(s):");
        foreach (var member in members)
        {
            PrintMemberLine(member);
        }
    }

    private void FindByDocument()
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento: ");
        try
        {
            PrintMemberDetails(memberService.GetByDocument(documentNumber));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void FindByName()
    {
        var namePart = ConsoleReader.ReadRequiredText("Nombre o parte del nombre a buscar: ");
        var members = memberService.SearchByName(namePart);

        if (members.Count == 0)
        {
            Console.WriteLine("No se encontraron asociados con ese nombre.");
            return;
        }

        Console.WriteLine($"{members.Count} coincidencia(s):");
        foreach (var member in members)
        {
            PrintMemberLine(member);
        }
    }

    private void UpdateMember()
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento del asociado a actualizar: ");

        Member current;
        try
        {
            current = memberService.GetByDocument(documentNumber);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Console.WriteLine("Deje el campo vacío para conservar el valor actual.");
        Console.WriteLine($"Nombre actual: {current.FullName}");
        var fullNameInput = ConsoleReader.ReadOptionalText("Nuevo nombre: ") ?? current.FullName;

        Console.WriteLine($"Teléfono actual: {current.PhoneNumber ?? "(sin registrar)"}");
        var phoneInput = ConsoleReader.ReadOptionalText("Nuevo teléfono: ") ?? current.PhoneNumber;

        Console.WriteLine($"Dirección actual: {current.Address ?? "(sin registrar)"}");
        var addressInput = ConsoleReader.ReadOptionalText("Nueva dirección: ") ?? current.Address;

        try
        {
            memberService.Update(documentNumber, fullNameInput, phoneInput, addressInput);
            Console.WriteLine("Datos actualizados correctamente.");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"No se pudo actualizar: {ex.Message}");
        }
    }

    private void DeleteMember()
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento del asociado a eliminar: ");

        Member current;
        try
        {
            current = memberService.GetByDocument(documentNumber);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        if (!ConsoleReader.ReadConfirmation($"¿Confirma que desea eliminar a {current.FullName}?"))
        {
            Console.WriteLine("Operación cancelada.");
            return;
        }

        try
        {
            memberService.Delete(documentNumber);
            Console.WriteLine("Asociado eliminado.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"No se pudo eliminar: {ex.Message}");
        }
    }

    private void ShowBalance()
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento: ");
        try
        {
            var member = memberService.GetByDocument(documentNumber);
            Console.WriteLine($"Saldo actual: {ConsoleFormat.Cop0(member.Balance)}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task ShowBalanceInUsdAsync()
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento: ");
        try
        {
            var balance = await memberService.GetBalanceInUsdAsync(documentNumber);
            Console.WriteLine($"Saldo en pesos: {ConsoleFormat.Cop0(balance.BalanceCop)}");
            Console.WriteLine($"TRM utilizada: {ConsoleFormat.Cop0(balance.ExchangeRate)} por dólar " +
                               $"(vigente del {ConsoleFormat.ShortDate(balance.RateValidFrom)} al {ConsoleFormat.ShortDate(balance.RateValidUntil)})");
            Console.WriteLine($"Saldo equivalente: {ConsoleFormat.Usd2(balance.BalanceUsd)}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void RegisterMovement(MovementType type)
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento: ");
        var label = type == MovementType.Deposit ? "consignar" : "retirar";
        var amount = ConsoleReader.ReadDecimal($"Valor a {label}: ");

        try
        {
            var movement = type == MovementType.Deposit
                ? movementService.RegisterDeposit(documentNumber, amount)
                : movementService.RegisterWithdrawal(documentNumber, amount);

            var member = memberService.GetByDocument(documentNumber);

            Console.WriteLine(movement.Type == MovementType.Deposit ? "Consignación registrada." : "Retiro registrado.");
            if (movement.Fee > 0)
                Console.WriteLine($"Se aplicó una comisión de manejo de efectivo de {ConsoleFormat.Cop0(movement.Fee)}.");

            Console.WriteLine($"Nuevo saldo: {ConsoleFormat.Cop0(member.Balance)}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Movimiento rechazado: {ex.Message}");
        }
    }

    private void ShowMovements()
    {
        var documentNumber = ConsoleReader.ReadRequiredText("Número de documento: ");

        IReadOnlyList<Movement> movements;
        string fullName;
        try
        {
            var member = memberService.GetByDocument(documentNumber);
            movements = member.Movements;
            fullName = member.FullName;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        if (movements.Count == 0)
        {
            Console.WriteLine("Este asociado todavía no tiene movimientos registrados.");
            return;
        }

        Console.WriteLine($"{movements.Count} movimiento(s) de {fullName}:");
        var runningBalance = 0m;
        foreach (var movement in movements)
        {
            runningBalance += movement.SignedEffect;
            var typeLabel = movement.Type == MovementType.Deposit ? "Consignación" : "Retiro";
            var feeText = movement.Fee > 0 ? $" (comisión {ConsoleFormat.Cop0(movement.Fee)})" : string.Empty;
            Console.WriteLine($"  {ConsoleFormat.DateTimeStamp(movement.OccurredAt)}  {typeLabel,-13} " +
                               $"{ConsoleFormat.Cop0(movement.Amount)}{feeText}  saldo: {ConsoleFormat.Cop0(runningBalance)}");
        }
    }

    private static void PrintMemberLine(Member member) =>
        Console.WriteLine($"  {member.DocumentNumber,-15} {member.FullName,-30} {ConsoleFormat.Cop0(member.Balance),15}");

    private static void PrintMemberDetails(Member member)
    {
        Console.WriteLine($@"Documento: {member.DocumentNumber}
Nombre: {member.FullName}
Telefono: {member.PhoneNumber ?? "(sin registrar)"}
Direccion: {member.Address ?? "(sin registrar)"}
Asociado desde: {ConsoleFormat.ShortDate(member.JoinedAt)}
Saldo actual: {ConsoleFormat.Cop0(member.Balance)}");
    }
}