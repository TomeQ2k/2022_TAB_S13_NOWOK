﻿using MediatR;
using Microsoft.AspNetCore.Http;

namespace FileStorage.API.Application.Features.PutFile
{
    public record PutFileCommand : IRequest<PutFileResponse>
    {
        public string Key { get; init; }
        public string Name { get; init; }

        public IFormFile File { get; init; }
    }
}