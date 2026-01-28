using SolicitudGastoApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolicitudGastoApp.Application.DTOs
{
    public class ApprovalResponseDto
    {
        public Guid Id { get; set; }
        public Guid ApproverEmployeeId { get; set; }
        public string ApproverName { get; set; } = "";
        public ApprovalDecision Decision { get; set; }
        public string? Comment { get; set; }
        public int StepNumber { get; set; }
        public DateTime DecidedAtUtc { get; set; }
    }
}
