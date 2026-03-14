using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using matcha.Modelo;

namespace matcha.Components
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public void MarkUserAsAuthenticated(Empleado empleado)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, empleado.EmpleadoID.ToString()),
                new Claim(ClaimTypes.Name, empleado.UserName ?? ""),
                new Claim(ClaimTypes.Email, empleado.Email ?? ""),
                new Claim(ClaimTypes.Role, empleado.RolNombre ?? "Psicologo")
            }, "CustomAuth", ClaimTypes.Name, ClaimTypes.Role);

            _currentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public void MarkUserAsLoggedOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}
