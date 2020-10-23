using SpiritWorld.Inventories;
using SpiritWorld.Inventories.Items;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SpiritWorld.Game.Controllers {
  public class PlayerPackGridController : MonoBehaviour {

    /// <summary>
    /// The tile texture for the grid.
    /// </summary>
    public Texture2D GridTileTexture;

    /// <summary>
    /// The background image for the item grid
    /// </summary>
    public AspectRatioFitter backgroundAspectRatio;

    /// <summary>
    /// dimensions for the texture
    /// </summary>
    public Coordinate GridTileDimensions 
      => (GridTileTexture.width, GridTileTexture.height);

    /// <summary>
    /// The background image for the item grid
    /// </summary>
    Image gridImage;

    /// <summary>
    /// The texture we use for the item grid
    /// </summary>
    Texture2D itemGridTexture;

    /// <summary>
    /// The aspect ratio filter
    /// </summary>
    AspectRatioFitter aspectRatioFitter;

    /// <summary>
    /// The inventory this manages for the local player
    /// </summary>
     ShapedPack packInventory
#if UNITY_EDITOR
      = new ShapedPack((4, 7), new (Item, Coordinate)[] {
        (new Item(Item.Types.Iron, 2), (0,0)),
        (new Item(Item.Types.Iron, 1), (2,2)),
        (new Item(Item.Types.Wood, 2), (3,2))
    });
#else
    => Universe.LocalPlayer.packInventory;
#endif

    /// <summary>
    /// consts and connections
    /// </summary>
    void Awake() {
      gridImage = GetComponent<Image>();
      aspectRatioFitter = GetComponent<AspectRatioFitter>();
    }

    /// <summary>
    /// Set up the grid based on player inventory
    /// </summary>
    void Start() {
      // make the image texture
      itemGridTexture = new Texture2D(
        GridTileTexture.width * packInventory.dimensions.x,
        GridTileTexture.height * packInventory.dimensions.y
      );
      aspectRatioFitter.aspectRatio = (float)packInventory.dimensions.x / (float)packInventory.dimensions.y;
      backgroundAspectRatio.aspectRatio = aspectRatioFitter.aspectRatio;
      itemGridTexture.filterMode = FilterMode.Point;

      // add each slot to the texture's pixel collection.
      //Color[,] pixels = new Color[itemGridTexture.width, itemGridTexture.height];
      // loop though each image spot in the grid
      Coordinate.Zero.until(packInventory.dimensions, textureGridLocation => {
        // loop though each pixel in that image.
        Coordinate.Zero.until(GridTileDimensions, localPixelOffset => {
          Coordinate test = localPixelOffset;
          (int x, int y) = (textureGridLocation * GridTileDimensions) + localPixelOffset;
          Color pixel = GridTileTexture.GetPixel(localPixelOffset.x, localPixelOffset.y);
          /*pixels[x, y] = pixel;*/
          itemGridTexture.SetPixel(x, y, pixel);
        });
      });

      // set all the pixels we got and make a sprite out of it
      //itemGridTexture.SetPixels(flatten(pixels));
      itemGridTexture.Apply();
      gridImage.sprite = Sprite.Create(
        itemGridTexture,
        new Rect(
          0,
          0,
          itemGridTexture.width,
          itemGridTexture.height
        ),
        new Vector2(0.5f, 0.5f)
      );
    }

    /// <summary>
    /// Flatten a color aray
    /// </summary>
    /// <param name="colors"></param>
    /// <returns></returns>
    Color[] flatten(Color[,] colors) {
      Color[] flattenedColors = new Color[colors.GetLength(0) * colors.GetLength(1)];
      int index = 0;
      for (int x = 0; x < colors.GetLength(0); x++) {
        for (int y = 0; y < colors.GetLength(1); y++) {
          flattenedColors[index++] = colors[x, y];
        }
      }

      return flattenedColors;
    }
  }
}
