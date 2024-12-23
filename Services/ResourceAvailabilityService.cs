using ReserveApp.Interfaces;


namespace ReserveApp.Services
{
  public class ResourceAvailabilityService : BackgroundService
  {
    private readonly IServiceProvider _serviceProvider;

    public ResourceAvailabilityService(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        await CheckAndSetResourceAvailabilityAsync();
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
      }
    }

    private async Task CheckAndSetResourceAvailabilityAsync()
    {
      using (var scope = _serviceProvider.CreateScope())
      {
        var adminService = scope.ServiceProvider.GetRequiredService<IAdminService>();

        var userResources = await adminService.GetAllUserResourcesAsync();
        foreach (var userResource in userResources)
        {
          if (userResource.TimeRemaining <= TimeSpan.Zero && userResource.Status == "Accepted")
          {
            await adminService.ChangeResourceStatusAsync(userResource, "Expired");
          }
        }
      }
    }
  }
}