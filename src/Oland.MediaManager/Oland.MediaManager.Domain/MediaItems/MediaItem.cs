using System.Text.Json.Serialization;
using Oland.MediaManager.Domain.MediaItems.App;
using Oland.MediaManager.Domain.MediaItems.Link;
using Oland.MediaManager.Domain.MediaItems.Movie;
using Oland.MediaManager.Domain.MediaItems.Music;
using Oland.MediaManager.Domain.MediaItems.Photo;
using Oland.MediaManager.Domain.MediaItems.Poll;
using Oland.MediaManager.Domain.MediaItems.Text;

namespace Oland.MediaManager.Domain.MediaItems;

/// <summary>
///     Базовый контракт для всех типов медиа
/// </summary>
[JsonDerivedType(typeof(PhotoMedia), "photo")]
[JsonDerivedType(typeof(MovieMedia), "movie")]
[JsonDerivedType(typeof(MusicMedia), "music")]
[JsonDerivedType(typeof(PollMedia), "poll")]
[JsonDerivedType(typeof(LinkMedia), "link")]
[JsonDerivedType(typeof(TextMedia), "text")]
[JsonDerivedType(typeof(AppMedia), "app")]
[JsonDerivedType(typeof(ProductMedia), "product")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
public abstract class MediaItem
{
}