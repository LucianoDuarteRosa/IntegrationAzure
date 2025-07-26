# Script PowerShell para recortar favicons
Add-Type -AssemblyName System.Drawing

function Crop-Favicon {
    param(
        [string]$InputPath,
        [string]$OutputPath,
        [double]$CropPercentage = 0.25
    )
    
    try {
        # Carregar a imagem
        $originalImage = [System.Drawing.Image]::FromFile($InputPath)
        
        # Calcular área de corte
        $cropMarginX = [int]($originalImage.Width * $CropPercentage)
        $cropMarginY = [int]($originalImage.Height * $CropPercentage)
        
        $cropRect = New-Object System.Drawing.Rectangle(
            $cropMarginX,
            $cropMarginY,
            ($originalImage.Width - 2 * $cropMarginX),
            ($originalImage.Height - 2 * $cropMarginY)
        )
        
        # Criar nova imagem cortada
        $croppedBitmap = New-Object System.Drawing.Bitmap($cropRect.Width, $cropRect.Height)
        $graphics = [System.Drawing.Graphics]::FromImage($croppedBitmap)
        
        # Definir qualidade alta
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
        $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        
        # Desenhar a imagem cortada
        $destRect = New-Object System.Drawing.Rectangle(0, 0, $cropRect.Width, $cropRect.Height)
        $graphics.DrawImage($originalImage, $destRect, $cropRect, [System.Drawing.GraphicsUnit]::Pixel)
        
        # Redimensionar para 32x32 (tamanho padrão de favicon)
        $favicon = New-Object System.Drawing.Bitmap(32, 32)
        $faviconGraphics = [System.Drawing.Graphics]::FromImage($favicon)
        $faviconGraphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $faviconGraphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
        
        $faviconRect = New-Object System.Drawing.Rectangle(0, 0, 32, 32)
        $faviconGraphics.DrawImage($croppedBitmap, $faviconRect)
        
        # Salvar
        $favicon.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        
        # Limpeza
        $graphics.Dispose()
        $faviconGraphics.Dispose()
        $croppedBitmap.Dispose()
        $favicon.Dispose()
        $originalImage.Dispose()
        
        Write-Host "Favicon criado: $OutputPath" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "Erro ao processar $InputPath`: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Diretórios
$publicDir = "c:\Users\Luciano Duarte\Desktop\Projetos\IntegrationAzure\Aplication\public"

Write-Host "Processando favicons..." -ForegroundColor Cyan
Write-Host "=" * 50

# Processar favicon.png
$favicon1Input = Join-Path $publicDir "favicon.png"
$favicon1Output = Join-Path $publicDir "favicon_cropped.png"

if (Test-Path $favicon1Input) {
    Write-Host "Processando favicon.png..." -ForegroundColor Yellow
    Crop-Favicon -InputPath $favicon1Input -OutputPath $favicon1Output -CropPercentage 0.25
} else {
    Write-Host "Arquivo nao encontrado: $favicon1Input" -ForegroundColor Red
}

Write-Host "-" * 30

# Processar favicon2.png  
$favicon2Input = Join-Path $publicDir "favicon2.png"
$favicon2Output = Join-Path $publicDir "favicon2_cropped.png"

if (Test-Path $favicon2Input) {
    Write-Host "Processando favicon2.png..." -ForegroundColor Yellow
    Crop-Favicon -InputPath $favicon2Input -OutputPath $favicon2Output -CropPercentage 0.25
} else {
    Write-Host "Arquivo nao encontrado: $favicon2Input" -ForegroundColor Red
}

Write-Host "=" * 50
Write-Host "Processamento concluido!" -ForegroundColor Green
Write-Host "Arquivos criados na pasta public/" -ForegroundColor Cyan
