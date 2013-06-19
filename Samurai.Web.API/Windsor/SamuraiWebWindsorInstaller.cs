using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Http;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.TypedFactory;

using Infrastructure.Data;
using Samurai.SqlDataAccess;
using Samurai.Domain.Repository;
using Samurai.Services;
using Samurai.SqlDataAccess.Mapping;
using Samurai.Domain.Value;
using Samurai.Domain.Value.Excel;
using Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Infrastructure;
using Samurai.Web.API.Infrastructure;
using Samurai.Web.API.Messaging.TennisSchedule;
using Samurai.Web.API.Controllers;

namespace Samurai.Web.API.Windsor
{
  public class SamuraiWebWindsorInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      var repositoryType = "irrelevant";
      var basePath = "not telling you";

      container.AddFacility<TypedFactoryFacility>();

      container.Register(Classes
                        .FromAssemblyContaining<FixturesController>()
                        .BasedOn<ApiController>()
                        .LifestylePerWebRequest());

      container.Register(Component
                        .For<IValueOptions>()
                        .ImplementedBy<ValueOptions>()
                        .LifeStyle
                        .PerWebRequest);

      container.Register(Component
                        .For<DbContext>()
                        .ImplementedBy<ValueSamuraiContext>()
                        .LifeStyle.PerWebRequest);

      container.Register(Component
                        .For<IWebRepositoryProvider>()
                        .ImplementedBy<WebRepositoryProvider>()
                        .DependsOn(new
                        {
                          repositoryType = repositoryType,
                          basePath = basePath
                        }
                        )
                        .LifeStyle
                        .PerWebRequest);

      container.Register(Component
                        .For<IBus>()
                        .ImplementedBy<MessageBus>()
                        .LifeStyle
                        .PerWebRequest);

      container.Register(Classes
                        .FromAssemblyContaining<SqlPredictionRepository>()
                        .Where(t => t.Name.StartsWith("Sql") &&
                                    t.Name.EndsWith("Repository"))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest());

      container.Register(Classes
                        .FromAssemblyContaining<FootballFixtureService>()
                        .Where(t => t.Name.EndsWith("Service"))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest());

      container.Register(Classes
                        .FromAssemblyContaining<GetTennisScheduleHandler>()
                        .Where(t => t.Name.EndsWith("Handler"))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest());

      container.Register(Classes
                        .FromAssemblyContaining<PredictionStrategyProvider>()
                        .Where(t => !t.Name.StartsWith("Excel"))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest());

      container.Register(Component
                        .For<IMessageHandlerFactory>()
                        .AsFactory()
                        .LifestylePerWebRequest());

      container.Register(Component
                        .For<ICommandHandlerFactory>()
                        .AsFactory()
                        .LifestylePerWebRequest());

      ProgressReporterProvider.Current = new HubProgressReporterProvider();
    }
  }
}