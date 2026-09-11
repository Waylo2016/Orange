using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.Guild;

namespace Orange.Api.Interfaces;

public interface IMeService
{
    public Task<ActionResult<List<GuildSummaryDto>>> GetGuildsAsync();
}