using System.Collections.Generic;

namespace ProjectBoss.Api.Dtos
{
    public class ChartDataDto
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public dynamic Extra { get; set; }
    }

    public class GroupedChartDataDto
    {
        public string Name { get; set; }
        public List<ChartDataDto> Series { get; set; }
    }    
}
