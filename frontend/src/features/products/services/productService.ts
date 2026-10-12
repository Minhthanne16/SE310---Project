import { apiClient } from '@/api/client';
import type { ApiResponse } from '@/types/api';
import type { Product, CreateProductDto, UpdateProductDto } from '@/types/product';

export const productService = {
  async getAll(): Promise<Product[]> {
    const res = await apiClient.get<ApiResponse<Product[]>>('/products');
    return res.data.data;
  },

  async getById(id: string): Promise<Product> {
    const res = await apiClient.get<ApiResponse<Product>>(`/products/${id}`);
    return res.data.data;
  },

  async create(data: CreateProductDto): Promise<Product> {
    const res = await apiClient.post<ApiResponse<Product>>('/products', data);
    return res.data.data;
  },

  async update(id: string, data: UpdateProductDto): Promise<Product> {
    const res = await apiClient.put<ApiResponse<Product>>(`/products/${id}`, data);
    return res.data.data;
  },

  async delete(id: string): Promise<boolean> {
    const res = await apiClient.delete<ApiResponse<boolean>>(`/products/${id}`);
    return res.data.data;
  },
};
