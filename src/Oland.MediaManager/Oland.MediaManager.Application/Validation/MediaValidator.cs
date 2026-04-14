using Oland.MediaManager.Application.Builders;
using Oland.MediaManager.Domain.MediaItems.Link;
using Oland.MediaManager.Domain.MediaItems.Photo;
using Oland.MediaManager.Domain.MediaItems.Poll;

namespace Oland.MediaManager.Application.Validation;

public class MediaValidator : IMediaValidator
{
    public ValidationResult Validate(MediaCollection collection)
    {
        var errors = new List<string>();
            
        foreach (var item in collection.Items)
        {
            errors.AddRange(item switch
            {
                //TODO: Сделать валидацию
                //PhotoMedia { List.Count: 0 } => ["Photo list cannot be empty"],
                PollMedia { Answers.Count: < 2 } => ["Poll must have at least 2 answers"],
                LinkMedia link when !Uri.IsWellFormedUriString(link.Url, UriKind.Absolute) 
                    => [$"Invalid URL: {link.Url}"],
                _ => []
            });
        }
            
        return new ValidationResult(errors);
    }
}