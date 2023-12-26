using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.User;
using BlazorSozluk.Common.Infrastructure;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.User.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IMapper mapper;
    private readonly IUserRepository userRepository;

    public CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository)
    {
        this.mapper = mapper;
        this.userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existUser = await userRepository.GetSingleAsync(i => i.EmailAddress == request.EmailAddress);

        if (existUser is not null)
            throw new DatabaseValidationException("User already exists!");

        var dbUser = mapper.Map<Domain.Models.User>(request);

        var rows = await userRepository.AddAsync(dbUser);

        // Email changed/created
        if (rows > 0)
        {
            var @event = new UserEmailChangedEvent()
            {
                OldEmailAddress = null,
                NewEmailAddress = request.EmailAddress,
            };

            QueueFactory.SendMessageToExchange(
                exchangeName: SozlukConstants.UserExchangeName,
                exchangeType: SozlukConstants.DefaultExchangeType,
                queueName: SozlukConstants.UserEmailChangedQueueName,
                obj: @event);
        }

        return dbUser.Id;
    }
}
