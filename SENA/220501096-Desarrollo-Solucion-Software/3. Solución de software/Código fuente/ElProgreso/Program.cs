using ElProgreso.Repositories;
using ElProgreso.Services;
using ElProgreso.UI;

// Shared HttpClient used to query the open data API (datos.gov.co) for exchange rates.
// A short timeout keeps the console responsive if the endpoint is slow or unreachable.
var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://www.datos.gov.co/"),
    Timeout = TimeSpan.FromSeconds(8)
};

// In-memory storage acting as the single source of truth for members during this session.
var memberRepository = new InMemoryMemberRepository();
var exchangeRateService = new ExchangeRateService(httpClient);
var memberService = new MemberService(memberRepository, exchangeRateService);
var movementService = new MovementService(memberRepository);
var reportService = new ReportService(memberRepository);

// Entry point UI: a console-based teller that wires the services above into user-facing menus.
var console = new TellerConsole(memberService, movementService, reportService);
await console.RunAsync();