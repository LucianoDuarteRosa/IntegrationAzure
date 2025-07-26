from PIL import Image
import os

def crop_favicon(input_path, output_path, crop_percentage=0.3):
    """
    Recorta uma imagem focando na parte central, removendo bordas vazias
    crop_percentage: porcentagem a ser removida de cada lado (0.3 = 30%)
    """
    try:
        # Abrir a imagem
        img = Image.open(input_path)
        
        # Converter para RGBA se necessário
        if img.mode != 'RGBA':
            img = img.convert('RGBA')
        
        # Obter dimensões
        width, height = img.size
        
        # Calcular área de corte (foca no centro)
        crop_margin_x = int(width * crop_percentage)
        crop_margin_y = int(height * crop_percentage)
        
        # Definir área de corte
        left = crop_margin_x
        top = crop_margin_y
        right = width - crop_margin_x
        bottom = height - crop_margin_y
        
        # Recortar a imagem
        cropped_img = img.crop((left, top, right, bottom))
        
        # Redimensionar para tamanhos padrão de favicon
        # Criar múltiplos tamanhos
        sizes = [16, 32, 48, 64, 128, 256]
        
        for size in sizes:
            resized = cropped_img.resize((size, size), Image.Resampling.LANCZOS)
            
            # Salvar com nome baseado no tamanho
            name, ext = os.path.splitext(output_path)
            size_output = f"{name}_{size}x{size}{ext}"
            resized.save(size_output, 'PNG', optimize=True)
            print(f"Criado: {size_output}")
        
        # Salvar também uma versão padrão (32x32)
        favicon_32 = cropped_img.resize((32, 32), Image.Resampling.LANCZOS)
        favicon_32.save(output_path, 'PNG', optimize=True)
        print(f"Favicon principal criado: {output_path}")
        
        return True
        
    except Exception as e:
        print(f"Erro ao processar {input_path}: {str(e)}")
        return False

# Diretórios
public_dir = r"c:\Users\Luciano Duarte\Desktop\Projetos\IntegrationAzure\Aplication\public"
assets_dir = r"c:\Users\Luciano Duarte\Desktop\Projetos\IntegrationAzure\Aplication\src\assets"

# Processar favicon.png
favicon1_input = os.path.join(assets_dir, "favicon.png")
favicon1_output = os.path.join(public_dir, "favicon_cropped.png")

# Processar favicon2.png
favicon2_input = os.path.join(assets_dir, "favicon2.png")
favicon2_output = os.path.join(public_dir, "favicon2_cropped.png")

print("Processando favicons...")
print("=" * 50)

if os.path.exists(favicon1_input):
    print(f"Processando favicon.png...")
    crop_favicon(favicon1_input, favicon1_output, 0.25)  # Remove 25% de cada lado
else:
    print(f"Arquivo não encontrado: {favicon1_input}")

print("-" * 30)

if os.path.exists(favicon2_input):
    print(f"Processando favicon2.png...")
    crop_favicon(favicon2_input, favicon2_output, 0.25)  # Remove 25% de cada lado
else:
    print(f"Arquivo não encontrado: {favicon2_input}")

print("=" * 50)
print("Processamento concluído!")
print("Arquivos criados na pasta public/")
