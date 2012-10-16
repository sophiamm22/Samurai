using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.TypedFactory;
using System.Data.Entity;

using Infrastructure.Data;
using Samurai.WebPresentationModel.Controllers;
using Samurai.SqlDataAccess;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Mapping;
using Samurai.WebPresentationModel.Messaging;
using Samurai.Services;

using Samurai.WebPresentationModel.Messaging.Fixtures.CommandHandlers;

namespace Samurai.Web.Windsor
{
  public class SamuraiWindsorInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.AddFacility<TypedFactoryFacility>();

      container.Register(Component
                        .For<DbContext>()
                        .ImplementedBy<ValueSamuraiContext>()
                        .LifeStyle.PerWebRequest);

      container.Register(Component
                        .For<IBus>()
                        .ImplementedBy<MessageBus>()
                        .LifeStyle.PerWebRequest);

      container.Register(AllTypes
                        .FromAssemblyContaining<AccountController>()
                        .BasedOn<IController>()
                        .LifestylePerWebRequest());

      container.Register(AllTypes
                        .FromAssemblyContaining<SqlFixtureRepository>()
                        .Where(t => t.Name.StartsWith("Sql") && 
                                    t.Name.EndsWith("Repository"))                        
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest());

      container.Register(AllTypes
                        .FromAssemblyContaining<FixtureService>()
                        .Where(t => t.Name.EndsWith("Service"))
                        .WithService
                        .AllInterfaces()
                        .LifestylePerWebRequest());

      container.Register(AllTypes
                        .FromAssemblyContaining<IndexFootballFixturesHandler>()
                        .Where(t => t.Name.EndsWith("Handler"))
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
    } 
  }
}