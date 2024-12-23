# MsixInstaller
一个 MSIX 包装器，包装 MSIX 以简化用户信任证书的步骤

目前仅支持中文

## 使用方法

```yaml
# ...
jobs:
  build:
# ...
    runs-on: windows-latest
    steps:
# ...
      - id: MsixInstaller
        uses: frg2089/MsixInstaller@v1
        with:
          msix: <path to your .msix>
# ...
```