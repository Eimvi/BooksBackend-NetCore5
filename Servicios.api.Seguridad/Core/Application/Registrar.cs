using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Servicios.api.Seguridad.Core.DTO;
using Servicios.api.Seguridad.Core.Entities;
using Servicios.api.Seguridad.Core.JwtLogic;
using Servicios.api.Seguridad.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios.api.Seguridad.Core.Application
{
    public class Registrar
    {
        public class UsuarioRegistrarCommand : IRequest<UsuarioDTO>
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UsuarioRegistrarHandler : IRequestHandler<UsuarioRegistrarCommand, UsuarioDTO>
        {
            private readonly SeguridadContexto _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IMapper _mapper;
            private readonly IJwtGenerator _jwtGenerator;
            public UsuarioRegistrarHandler(SeguridadContexto context, UserManager<Usuario> userManager, IMapper mapper, IJwtGenerator jwtGenerator)
            {
                _context = context;
                _userManager = userManager;
                _mapper = mapper;
                _jwtGenerator = jwtGenerator;
            }

            public class UsuarioRegistrarValidar : AbstractValidator<UsuarioRegistrarCommand>
            {
                public UsuarioRegistrarValidar()
                {
                    RuleFor(x => x.Nombre).NotEmpty();
                    RuleFor(x => x.Apellido).NotEmpty();
                    RuleFor(x => x.Username).NotEmpty();
                    RuleFor(x => x.Email).NotEmpty();
                    RuleFor(x => x.Password).NotEmpty();
                }
            }
            public async Task<UsuarioDTO> Handle(UsuarioRegistrarCommand request, CancellationToken cancellationToken)
            {
                var exist = await _context.Users.Where( x => x.Email == request.Email).AnyAsync();
                if (exist)
                {
                    throw new Exception("El email del usuario ya existe.");
                }

                exist = await _context.Users.Where(x => x.UserName == request.Username).AnyAsync();
                if (exist)
                {
                    throw new Exception("El UserName del usuario ya existe.");
                }

                var usuario = new Usuario
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    Email = request.Email,
                    UserName = request.Username
                };

                var result = await _userManager.CreateAsync(usuario, request.Password);
                if (result.Succeeded)
                {
                    var usuarioDto = _mapper.Map<Usuario, UsuarioDTO>(usuario);
                    usuarioDto.Token = _jwtGenerator.CreateToken(usuario);
                    return usuarioDto;
                }
                throw new Exception("No se pudo registrar el usuario.");

            }
        }
    }
}
