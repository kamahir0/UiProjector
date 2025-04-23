# UiProjector
A Unity library for projecting world-space objects onto the screen and attaching UI elements that follow them.

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)
![unity-version](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![releases](https://img.shields.io/github/release/kamahir0/UiProjector.svg)](https://github.com/kamahir0/UiProjector/releases)

UiProjectorは、ゲームによくある「3Dオブジェクトに追従するUI」を簡単に実装するための汎用的なシステムです。

## 使用方法
### 最も簡単な使い方
```
// UIと追従対象にするTransformを渡す
// UiHandleという構造体が返される
UiHandle uiHandle = ProjectionUi.AddUi(uiRectTransform, targetTarnsform);

// 不要になったらReleaseを呼び出して消す
uiHandle.Release();
```

システムを利用開始するためのメソッドは全て`ProjectionUi`というstaticクラスに集約されています。
このAddUiメソッドにはオーバーロードが色々あり、この呼び出し方は最も引数を省略しているシンプルなものです。
省略一切無しの場合は以下の様になっています。

```
/// <summary>
/// UIを追加する。デフォルトCanvasを使用する
/// </summary>
/// <param name="ui">追従させるUIのRectTransform</param>
/// <param name="target">追従対象のTransform</param>
/// <param name="worldSpaceOffset">ワールド空間でのオフセット</param>
/// <param name="screenSpaceOffset">スクリーン空間でのオフセット</param>
/// <param name="reviser">UIの位置を修正するProjectionReviser</param>
/// <param name="releaseAction">UiHandle.Release実行時の処理（未指定ならDestroy）</param>
public static UiHandle AddUi(RectTransform ui, Transform target, Vector3 worldSpaceOffset, Vector2 screenSpaceOffset, IProjectionReviser reviser = null, Action<RectTransform> releaseAction = null)
```

`IProjectionReviser`については後述。
`releaseAction`にはReleaseされたときUIに対して施す処理を渡せます。デフォルト値がnullになっていますが、nullの場合`はUnityEngine.Object.Destroy`で処理されます。
この引数の使い道の想定はやはり、オブジェクトプールへの返却です。

```
ObjectPool<RectTransform> _pool;

// Release時にオブジェクトプールへ返却するように
ProjectionUi.AddUi(ui, target, releaseAction = _pool.Release);
```

このようにすることで、UiHandle.Releaseを呼び出すとUIがオブジェクトプールへと返却されるようになります。

### Canvasを分ける
前段の最も簡単な使い方を読んでいて「UIはどのCanvasに配置されるのか？」と疑問を持たれたかもしれません。
UiProjectorはゲーム開始時に『デフォルトCanvas』をDontDestroyOnLoadに生成し、先ほどのようなCanvasを指定しない簡単な呼び出し方ではデフォルトCanvasが使用されます。

もちろんデフォルトCanvasを使わない方法も用意されています。

```
// Canvasと、UIを描画するカメラを渡す
// CanvasHandleという構造体が返される
CanvasHandle canvasHandle = ProjectionUi.CreateCanvas(originalCanvas, camera);

// UIと追従対象にするTransformを渡す
// UiHandle構造体が返される
UiHandle uiHandle = canvasHandle.AddUi(uiRectTransform, targetTarnsform, ...);

// 不要になったらDisposeを呼び出して破棄する（配下のUIは全てReleaseされ、Canvasも破棄される）
canvasHandle.Dispose();
```

`CreateCanvas`を呼び出すことでCanvasを作成できます。
第一引数`Canvas original`は作成するCanvasの複製元になります。一応Prefabを渡す想定で、作成するCanvasのパラメータはそのPrefabで予め設定しておいてもらうのが自然かと思います。
第二引数`Camera camera`はUIを描画するカメラを渡します。CanvasのRenderModeが`Overlay`であればメインカメラをそのまま。`CameraSpace`であればUIカメラを渡します。

CanvasHandle構造体にはAddUiメソッドがあります。前段で紹介した`ProjectionUi.AddUi`と使い方は全く同じです。
IDisposableを実装しており、DisposeすることでCanvasを破棄できます。また、このときCanvasを破棄する直前にAddUiした全てのUIをReleaseします。

### UIを画面内に収める
ゲームによくある「3Dオブジェクトに追従するUI」において、追従対象が画面外へと消えてしまったときのUI側の挙動はケースバイケースです。
そのままUIも一緒に消えてしまうこともある一方で、UIだけは画面内に収まり続けるみたいなケースも多いです。

UiProjectorではそのような挙動の実現もサポートしています。

```
// IProjectionReviserを実装しているScreenReviserクラス
ScreenReviser _screenReviser = new();

// ScreenReviserを渡すことでスクリーン内にUIを収めてくれる
canvasHandle.AddUi(ui, target, _screenReviser);
```

AddUiメソッドの引数のうちの一つ`IProjectionReviser reviser`に、`IProjectionReviser`インターフェースを実装したクラスのインスタンスを渡します。
`IProjectionReviser`インターフェースは、UIの位置を修正するためのインターフェースです。

UiProjectorには標準で以下のクラスが用意されています
- **ScreenReviser**
  - 
- **SafeAreaReviser**
  - 
- **RectReviser**
  - 
