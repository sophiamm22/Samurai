﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.TypedFactory;
using System.Data.Entity;

using Infrastructure.Data;
using Samurai.SqlDataAccess;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Mapping;
using Samurai.Services;
using Samurai.Domain.Value;
using Samurai.Domain.Value.Excel;
using Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Infrastructure;
using Samurai.Web.API.Infrastructure;
using Samurai.Web.API.Messaging.TennisSchedule;

namespace Samurai.Sandbox
{
  public class SamuraiSandboxWindsorInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      var repositoryType = "SaveTestData";
      var basePath = @"D:\My Box Files\";

      container.AddFacility<TypedFactoryFacility>();

      container.Register(Component
                        .For<IValueOptions>()
                        .ImplementedBy<ValueOptions>());

      container.Register(Component
                        .For<DbContext>()
                        .ImplementedBy<ValueSamuraiContext>());

      container.Register(Component
                        .For<IWebRepositoryProvider>()
                        .ImplementedBy<WebRepositoryProvider>()
                        .DependsOn(new
                        {
                          repositoryType = repositoryType,
                          basePath = basePath
                        }
                        ));

      container.Register(Classes
                        .FromAssemblyContaining<SqlPredictionRepository>()
                        .Where(t => t.Name.StartsWith("Sql") &&
                                    t.Name.EndsWith("Repository"))
                        .WithService
                        .AllInterfaces());

      container.Register(Classes
                        .FromAssemblyContaining<FootballFixtureService>()
                        .Where(t => t.Name.EndsWith("Service"))
                        .WithService
                        .AllInterfaces());

      container.Register(Classes
                        .FromAssemblyContaining<GetTennisScheduleHandler>()
                        .Where(t => t.Name.EndsWith("Handler"))
                        .WithService
                        .AllInterfaces());

      container.Register(Classes
                        .FromAssemblyContaining<PredictionStrategyProvider>()
                        .Where(t => !t.Name.StartsWith("Excel"))
                        .WithService
                        .AllInterfaces());

      container.Register(Component
                        .For<IMessageHandlerFactory>()
                        .AsFactory());

      container.Register(Component
                        .For<ICommandHandlerFactory>()
                        .AsFactory());

      ProgressReporterProvider.Current = new ConsoleProgressReporterProvider();
    }
  }
}
