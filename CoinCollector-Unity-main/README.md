# Coin Collector 3D

Pràctica UF2 – Nucli del joc. Una bola que es mou amb física per un nivell, recull monedes i torna a començar si cau a la KillZone.

**Unity 6 LTS (6000.3)** · **Controls:** WASD / fletxes

## Contingut

- `Scenes/` – `MenuPrincipal` (botó Play) i `Level1`
- `Scripts/` – `PlayerController`, `Coin`, `KillZone`, `GameManager`, `MenuController`
- `Scripts/` (ampliacions) – `Spinner` (gir en Y) i `CoinFloat` (puja i baixa amb `Mathf.PingPong`)
- `Prefabs/` – `Coin`
- `Materials/` – terra, obstacles, bola i moneda

## El nivell

- Terra de 24×16 (`Floor`, tag `Ground`) amb tres pilars com a obstacles.
- Quatre límits invisibles (`Is Trigger`) i una `KillZone` a sota que reinicia l'escena.
- Deu monedes en forma de gemma repartides en ziga-zaga.

## Ús d'IA

Per fer aquesta pràctica he fet servir IA com a ajuda per escriure els scripts, generar les escenes amb un script d'editor (`Assets/Editor/LevelSetup.cs`) i redactar aquesta documentació.
