using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CineSpectra.Domain.Common;

namespace CineSpectra.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; } = string.Empty; 
    public ICollection<Show> Shows { get; set; } = new List<Show>(); 
}