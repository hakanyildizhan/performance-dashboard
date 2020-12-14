using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerformanceDashboard.Entity.Entity
{
    public class TestRun
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual TestScenario Scenario { get; set; }

        public virtual TestConfiguration Configuration { get; set; }

        public double Result { get; set; }

        public DateTime Date { get; set; }
    }
}
