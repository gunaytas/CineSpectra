using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CineSpectra.Domain.Common;
using CineSpectra.Domain.Enums;

namespace CineSpectra.Domain.Entities
{
    public class RatingCriteria : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public CriteriaTarget Target { get; set; }
    }
}
