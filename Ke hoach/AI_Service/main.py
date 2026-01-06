from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import fitz # PyMuPDF
import os
from typing import cast

app = FastAPI()

class AnalysisRequest(BaseModel):
    document_id : str
    file_path: str # Path to the PDF file
    
@app.post("/analyze")
async def analyze_document(request: AnalysisRequest):
    # kiem tra file ton tai
    if not os.path.isfile(request.file_path):
        raise HTTPException(status_code=404, detail="khong tim thay tap tin tren shared volume")
    
    try:
        # trich xuat van ban tu file PDF
        with fitz.open(request.file_path) as doc:
            text_parts: list[str] = []
            for page in doc:
                text_parts.append(cast(str, page.get_text("text")))
            text = "".join(text_parts)
        
        # gia lap ket qua ai ( de demo nhanh chua can api key)
        
        return {
            "document_id": request.document_id,
            "status": "Success",
            "extractedTextPreview": text[:200], # preview 200 ky tu
            "comlianceScore": 85.5, # gia tri gia lap
            "findings": "Phat hien thieu dieu khoan quyen xoa du lieu  theo ccpa."
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Loi khi xu ly tai lieu: {str(e)}")
