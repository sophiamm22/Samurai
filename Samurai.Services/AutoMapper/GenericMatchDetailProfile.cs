using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Model;

namespace Samurai.Services.AutoMapper
{
  public class GenericMatchDetailProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<GenericMatchDetailQuery, GenericMatchDetail>().IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchDetailQuery, GenericMatchDetail>().ForMember(x => x.MatchIdentifier, opt =>
        { opt.ResolveUsing<MatchIdentifierResolver>(); });
    }
  }

  public class MatchIdentifierResolver : ValueResolver<GenericMatchDetailQuery, string>
  {
    protected override string ResolveCore(GenericMatchDetailQuery source)
    {
      var haveFirstNames = !(string.IsNullOrEmpty(source.PlayerAFirstName) && string.IsNullOrEmpty(source.PlayerBFirstName));
      var teamPlayerA = haveFirstNames ? string.Format("{0},{1}", source.TeamOrPlayerA, source.PlayerAFirstName) : source.TeamOrPlayerA;
      var teamPlayerB = haveFirstNames ? string.Format("{0},{1}", source.TeamOrPlayerB, source.PlayerBFirstName) : source.TeamOrPlayerB;

      return string.Format("{0}/vs/{1}/{2}/{3}", teamPlayerA, teamPlayerB, source.TournamentEventName, source.MatchDate.ToShortDateString().Replace("/", "-"));
    }
  }
}
