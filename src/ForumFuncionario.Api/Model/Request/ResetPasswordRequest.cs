﻿namespace ForumFuncionario.Api.Model.Request
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }

    }
}
