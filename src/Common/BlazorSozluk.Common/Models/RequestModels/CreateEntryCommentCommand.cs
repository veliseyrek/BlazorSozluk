using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Common.Models.RequestModels
{
    public class CreateEntryCommentCommand:IRequest<Guid>
    {
        public Guid EntryId { get; set; }
        public string Content { get; set; }
        public Guid? CreateById { get; set; }
        public CreateEntryCommentCommand(Guid entryId, string content, Guid createdBy)
        {
            EntryId = entryId;
            Content = content;
            CreateById = createdBy;
        }
    }
}
