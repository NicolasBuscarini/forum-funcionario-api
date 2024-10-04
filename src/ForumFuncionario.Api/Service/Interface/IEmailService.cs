﻿namespace ForumFuncionario.Api.Service.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}