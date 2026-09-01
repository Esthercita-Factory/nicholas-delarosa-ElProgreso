using ElProgreso.Repositories;
using ElProgreso.Services;
using ElProgreso.UI;

var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://www.datos.gov.co/"),
    Timeout = TimeSpan.FromSeconds(8)
};

var memberRepository = new InMemoryMemberRepository();
var exchangeRateService = new ExchangeRateService(httpClient);
var memberService = new MemberService(memberRepository, exchangeRateService);
var movementService = new MovementService(memberRepository);
var reportService = new ReportService(memberRepository);

var console = new TellerConsole(memberService, movementService, reportService);
await console.RunAsync();