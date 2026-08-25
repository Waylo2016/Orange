using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Orange.Api.Constraints;

public class ULongRouteConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out object? value) || value is null)
            return false;

        return ulong.TryParse(value.ToString(), out _);
    }
}