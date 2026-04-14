using Oland.MediaManager.Domain.MediaItems;
using Oland.MediaManager.Domain.MediaItems.App;
using Oland.MediaManager.Domain.MediaItems.Link;
using Oland.MediaManager.Domain.MediaItems.Movie;
using Oland.MediaManager.Domain.MediaItems.Music;
using Oland.MediaManager.Domain.MediaItems.Photo;
using Oland.MediaManager.Domain.MediaItems.Poll;
using Oland.MediaManager.Domain.MediaItems.Text;

namespace Oland.MediaManager.Application.Builders;

/// <summary>
///     Конструктор для сборки коллекции медиа-объектов
/// </summary>
public class MediaCollectionBuilder
{
    private readonly List<MediaItem> _media = [];

    public MediaCollectionBuilder AddPhoto(Action<PhotoBuilder>? configure = null)
    {
        var builder = new PhotoBuilder();
        configure?.Invoke(builder);
        _media.Add(builder.Build());
        return this;
    }

    public MediaCollectionBuilder AddMovie(params string[] movieIds)
    {
        var media = new MovieMedia();
        media.List.AddRange(movieIds.Select(id => new MovieItem { Id = id }));
        _media.Add(media);
        return this;
    }

    public MediaCollectionBuilder AddMusic(Action<MusicBuilder> configure)
    {
        var builder = new MusicBuilder();
        configure(builder);
        _media.Add(builder.Build());
        return this;
    }

    public MediaCollectionBuilder AddPoll(string question, Action<PollBuilder> configure)
    {
        var builder = new PollBuilder { Question = question };
        configure(builder);
        _media.Add(builder.Build());
        return this;
    }

    public MediaCollectionBuilder AddLink(string url)
    {
        _media.Add(new LinkMedia { Url = url });
        return this;
    }

    public MediaCollectionBuilder AddText(string? text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            _media.Add(new TextMedia { Text = text });
        }
        
        return this;
    }

    public MediaCollectionBuilder AddApp(Action<AppBuilder>? configure = null)
    {
        var builder = new AppBuilder();
        configure?.Invoke(builder);
        _media.Add(builder.Build());
        return this;
    }

    public MediaCollectionBuilder AddProduct(decimal price, string? partnerLink = null, int lifetime = 90,
        string? currency = null)
    {
        var product = new ProductMedia()
        {
            Currency = currency,
            Lifetime = lifetime,
            PartnerLink = partnerLink ,
            Price = price
        };
        
        _media.Add(product);

        return this;
    }

    public MediaCollection Build()
    {
        return new MediaCollection(_media.ToList());
    }

    // Вспомогательные билдеры
    public class PhotoBuilder
    {
        private readonly List<PhotoItem> _items = new();

        public PhotoBuilder AddById(string id)
        {
            _items.Add(new PhotoItem { Id = id });
            return this;
        }

        internal PhotoMedia Build()
        {
            return new PhotoMedia { List = _items };
        }
    }

    public class MusicBuilder
    {
        private readonly List<MusicItem> _items = new();

        public MusicBuilder AddTrack(string id, string? title = null,
            string? artist = null, string? album = null)
        {
            _items.Add(new MusicItem
            {
                Id = id,
                Title = title,
                ArtistName = artist,
                AlbumName = album
            });
            return this;
        }

        internal MusicMedia Build()
        {
            return new MusicMedia { List = _items };
        }
    }

    public class PollBuilder
    {
        private readonly List<PollAnswer> _answers = new();
        public string Question { get; internal set; } = string.Empty;
        public string? Options { get; set; }

        public PollBuilder AddAnswer(string text)
        {
            _answers.Add(new PollAnswer { Text = text });
            return this;
        }

        public PollBuilder WithOptions(params string[] options)
        {
            Options = string.Join(",", options);
            return this;
        }

        internal PollMedia Build()
        {
            return new PollMedia
            {
                Question = Question,
                Answers = _answers,
                Options = Options
            };
        }
    }

    public class AppBuilder
    {
        private readonly List<AppAction> _actions = new();
        private readonly List<AppImage> _images = new();
        public string? Text { get; set; }

        public AppBuilder WithText(string text)
        {
            Text = text;
            return this;
        }

        public AppBuilder AddImage(string url, string? mark = null, string? title = null)
        {
            _images.Add(new AppImage { Url = url, Mark = mark, Title = title });
            return this;
        }

        public AppBuilder AddAction(string text, string? mark = null)
        {
            _actions.Add(new AppAction { Text = text, Mark = mark });
            return this;
        }

        internal AppMedia Build()
        {
            return new AppMedia
            {
                Text = Text,
                Images = _images,
                Actions = _actions
            };
        }
    }
}