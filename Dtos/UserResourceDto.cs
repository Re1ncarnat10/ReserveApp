﻿namespace ReserveApp.Dtos;

public class UserResourceDto
{
  public int UserResourceId { get; set; }
  public string UserId { get; set; }
  public int ResourceId { get; set; }
  public DateTime RentalStartTime { get; set; }
  public TimeSpan RentalDuration { get; set; }
  public TimeSpan TimeRemaining { get; set; }
}