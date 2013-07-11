using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class OutcomeComment : BaseEntity
  {
    public OutcomeComment()
    {
      this.ObservedOutcomes = new List<ObservedOutcome>();
    }

    public string Comment { get; set; }
    public virtual ICollection<ObservedOutcome> ObservedOutcomes { get; set; }
  }
}
