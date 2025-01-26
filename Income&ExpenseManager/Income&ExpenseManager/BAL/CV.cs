namespace Income_ExpenseManager.BAL
{
    public class CV
    {
        private static readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();

        public static int UserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("userId");
            return int.TryParse(userIdString, out var userId) ? userId : 0;
        }
    }
}