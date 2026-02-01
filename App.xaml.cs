using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjektuppgiftBudgetplanerare.Data;
using ProjektuppgiftBudgetplanerare.Repositories;
using ProjektuppgiftBudgetplanerare.ViewModels;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace ProjektuppgiftBudgetplanerare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BudgetPlanner",
            "budget.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            services.AddDbContext<BudgetDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddTransient<TransactionsViewModel>();

            Services = services.BuildServiceProvider();

            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();
                db.Database.EnsureCreated();
            }

            var mainWindow = new MainWindow();
            mainWindow.DataContext = Services.GetRequiredService<TransactionsViewModel>();
            mainWindow.Show();
        }
    }

}
