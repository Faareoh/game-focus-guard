#Requires -Version 7
<#
.SYNOPSIS
    Génère icon.ico (256/64/48/32/16 px) pour Game Focus Guard.
    Style : épingle de carte (map pin) violette sur fond gaming sombre.
.USAGE
    pwsh -NoProfile -ExecutionPolicy Bypass -File .\FocusTool.Ui\generate-icon.ps1
    Le fichier icon.ico est écrit dans FocusTool.Ui\.
#>

Add-Type -AssemblyName System.Drawing

# ── Couleurs ──────────────────────────────────────────────────────────────────
$colorBg        = [System.Drawing.Color]::FromArgb(255,  15,  15,  30)   # #0f0f1e
$colorPin       = [System.Drawing.Color]::FromArgb(255, 147,  51, 234)   # #9333ea
$colorPinLight  = [System.Drawing.Color]::FromArgb(255, 192, 132, 252)   # #c084fc
$colorHighlight = [System.Drawing.Color]::FromArgb(160, 255, 255, 255)
$colorHole      = [System.Drawing.Color]::FromArgb(200,  60,  10, 100)

function Add-RoundedRect {
    param([System.Drawing.Drawing2D.GraphicsPath]$path, [float]$x, [float]$y, [float]$w, [float]$h, [float]$r)
    $path.AddArc($x,           $y,           $r*2, $r*2, 180, 90)
    $path.AddArc($x+$w-$r*2,  $y,           $r*2, $r*2, 270, 90)
    $path.AddArc($x+$w-$r*2,  $y+$h-$r*2,  $r*2, $r*2,   0, 90)
    $path.AddArc($x,           $y+$h-$r*2,  $r*2, $r*2,  90, 90)
    $path.CloseFigure()
}

function New-PinBitmap {
    param([int]$sz)

    $bmp = New-Object System.Drawing.Bitmap($sz, $sz, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g   = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode      = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode  = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.PixelOffsetMode    = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    $s = [float]$sz   # scale (design reference = sz, coords below are fractions of sz)

    # ── Fond arrondi ──────────────────────────────────────────────────────────
    $bgPath = New-Object System.Drawing.Drawing2D.GraphicsPath
    $margin = [float]($sz * 0.02)
    $radius = [float]($sz * 0.18)
    Add-RoundedRect $bgPath $margin $margin ($s - 2*$margin) ($s - 2*$margin) $radius

    $bgBrush = New-Object System.Drawing.SolidBrush($colorBg)
    $g.FillPath($bgBrush, $bgPath)

    # ── Épingle (map pin) ─────────────────────────────────────────────────────
    # Tête du pin : cercle, centre à (0.5, 0.36)·sz, rayon 0.23·sz
    $hcx  = [float]($sz * 0.50)
    $hcy  = [float]($sz * 0.36)
    $hr   = [float]($sz * 0.23)
    # Corps : triangle, pointe à (0.5, 0.88)·sz
    $tipY = [float]($sz * 0.88)
    $bW   = [float]($sz * 0.14)   # demi-largeur base du triangle
    $bY   = [float]($hcy + $hr * 0.55)

    # Halo/lueur derrière le pin (gaming glow)
    if ($sz -ge 32) {
        $glowBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(45, 200, 100, 255))
        $gr = [float]($hr * 1.35)
        $g.FillEllipse($glowBrush, $hcx - $gr, $hcy - $gr, $gr*2, $gr*2)
        $glowBrush.Dispose()
    }

    # Gradient brush pour la tête (clair en haut, foncé en bas)
    $gradRect = New-Object System.Drawing.RectangleF(($hcx - $hr), ($hcy - $hr), ($hr*2), ($hr*2))
    $headGrad = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        $gradRect, $colorPinLight, $colorPin,
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical)

    # Corps (triangle) — même couleur de base
    $bodyPts = [System.Drawing.PointF[]]@(
        [System.Drawing.PointF]::new($hcx - $bW, $bY),
        [System.Drawing.PointF]::new($hcx + $bW, $bY),
        [System.Drawing.PointF]::new($hcx, $tipY)
    )
    $bodyBrush = New-Object System.Drawing.SolidBrush($colorPin)
    $g.FillPolygon($bodyBrush, $bodyPts)

    # Tête (cercle avec dégradé)
    $g.FillEllipse($headGrad, $hcx - $hr, $hcy - $hr, $hr*2, $hr*2)

    # Reflet sur la tête
    if ($sz -ge 24) {
        $hlBrush = New-Object System.Drawing.SolidBrush($colorHighlight)
        $hlW = [float]($hr * 0.65)
        $hlH = [float]($hr * 0.50)
        $g.FillEllipse($hlBrush, $hcx - $hr*0.50, $hcy - $hr*0.68, $hlW, $hlH)
        $hlBrush.Dispose()
    }

    # Trou central (donne du relief)
    if ($sz -ge 24) {
        $holeBrush = New-Object System.Drawing.SolidBrush($colorHole)
        $holeR = [float]($hr * 0.20)
        $g.FillEllipse($holeBrush, $hcx - $holeR, $hcy - $holeR, $holeR*2, $holeR*2)
        $holeBrush.Dispose()
    }

    # Contour fin pour mieux ressortir sur fond clair
    if ($sz -ge 32) {
        $outlinePen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(60, 255, 255, 255), [float]($sz * 0.015))
        $g.DrawEllipse($outlinePen, $hcx - $hr, $hcy - $hr, $hr*2, $hr*2)
        $outlinePen.Dispose()
    }

    $g.Dispose()
    $bgBrush.Dispose()
    $bodyBrush.Dispose()
    $headGrad.Dispose()
    $bgPath.Dispose()

    return $bmp
}

function Save-Ico {
    param([string]$outputPath, [int[]]$sizes)

    # Générer les PNG dans un tableau parallèle (pas de hashtable à clés entières)
    $pngDataList = [System.Collections.Generic.List[byte[]]]::new()
    foreach ($sz in $sizes) {
        $bmp = New-PinBitmap -sz $sz
        $ms  = New-Object System.IO.MemoryStream
        $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
        $pngDataList.Add($ms.ToArray())
        $ms.Dispose()
        $bmp.Dispose()
    }

    # Format ICO : header (6 o) + répertoire (16 o × N) + données PNG
    $fs = [System.IO.File]::OpenWrite($outputPath)
    $bw = New-Object System.IO.BinaryWriter($fs)

    # En-tête ICO
    $bw.Write([uint16]0)               # réservé
    $bw.Write([uint16]1)               # type : ICO
    $bw.Write([uint16]$sizes.Count)

    $offset = [int](6 + 16 * $sizes.Count)

    for ($i = 0; $i -lt $sizes.Count; $i++) {
        $sz   = $sizes[$i]
        $data = $pngDataList[$i]
        $bw.Write([byte]$(if ($sz -ge 256) { 0 } else { $sz }))  # width  (0 = 256)
        $bw.Write([byte]$(if ($sz -ge 256) { 0 } else { $sz }))  # height
        $bw.Write([byte]0)             # couleurs dans la palette
        $bw.Write([byte]0)             # réservé
        $bw.Write([uint16]1)           # plans de couleur
        $bw.Write([uint16]32)          # bits par pixel
        $bw.Write([uint32]$data.Length)
        $bw.Write([uint32]$offset)
        $offset += $data.Length
    }

    foreach ($data in $pngDataList) {
        $bw.Write($data)
    }

    $bw.Close()
    $fs.Close()
}

# ── Génération ────────────────────────────────────────────────────────────────
$out  = Join-Path $PSScriptRoot "icon.ico"
$sizes = @(256, 64, 48, 32, 16)

Save-Ico -outputPath $out -sizes $sizes

$sizeKb = [math]::Round((Get-Item $out).Length / 1KB, 1)
Write-Host "✓ Icône générée : $out ($sizeKb Ko, tailles : $($sizes -join ', ')px)"
