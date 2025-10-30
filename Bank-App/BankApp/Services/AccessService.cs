namespace BankApp.Services
{
    /// <summary>
    /// Service for managing application access control and login status.
    /// Required password for VG is 'VG2024'.
    /// </summary>
    public class AccessService
    {
        // Hardcoded password for the VG feature (security measure)
        private const string RequiredPassword = "VG2024";

        /// <summary>
        /// Indicates if the user is currently logged in and has access to the application.
        /// </summary>
        public bool IsLoggedIn { get; private set; } = false;

        /// <summary>
        /// Event triggered when the login status changes. Used by MainLayout/AccessGuard to re-render.
        /// </summary>
        public event Action OnChange;

        /// <summary>
        /// Attempts to log the user into the application.
        /// </summary>
        /// <param name="password">The password entered by the user.</param>
        /// <returns>True if the password matches, otherwise false.</returns>
        public bool Login(string password)
        {
            if (password == RequiredPassword)
            {
                IsLoggedIn = true;
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}