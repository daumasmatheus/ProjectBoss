using ProjectBoss.Api.Dtos.Enums;

namespace ProjectBoss.Api.Services.Helpers
{
    public static class EnumHelpers
    {
        public static string GetPriorityString(int priorityId)
        {
            switch ((EPriorities)priorityId)
            {
                case EPriorities.Normal:
                    return "Normal";
                case EPriorities.High:
                    return "Alta";
                case EPriorities.Low:
                    return "Baixa";
                default:
                    return string.Empty;
            }
        }

        public static string GetStatusString(int statusId)
        {
            switch ((EStatus)statusId)
            {
                case EStatus.Planned:
                    return "Planejada";
                case EStatus.InProgress:
                    return "Em Progresso";
                case EStatus.OnHold:
                    return "Em Espera";
                case EStatus.Finished:
                    return "Finalizada";
                default:
                    return string.Empty;
            }
        }
    }
}
