# MassContourStream

O **MassContourStream** é um projeto que fiz para me aprofundar em processamento de imagens utilizando OpenCV no .NET Core. Nele conseguimos fazer a captura do stream camera utilizando RTSP, renderizar em tempo real o stream, criar um laço virtual e detectar o tracejado de novos contornos devido à subtração de background que captura massas novas de pixels na região de interesse.

## Funcionalidades

- Captura do stream da câmera via RTSP
- Captura de frames via EMGU da OpenCV
- Renderização de frames processadas em tempo real em uma picture box
- Criação de polígonos (laços virtuais) para definir uma região de interesse para contorno de novas massas
- Subtração de imagem para detectar novos movimentos no polígono (laço virtual)
- Tracejado de novos contornos a partir das movimentações detectadas

## Como usar

1. Clone este repositório.
2. Abra o projeto no Visual Studio
3. Garanta que a dependencia OpenCVSharp esteja instalada
4. Configure a URL RTSP em Form1
5. Execute o projeto
6. Clique em pontos do stream até traçar um laço virtual
7. Observe a detecção de movimento dentro da região especificada.
