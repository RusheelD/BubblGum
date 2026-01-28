#include "runtime.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

// ==================== PRINT FUNCTIONS ====================

void bbgm_print_sugar(Sugar value)
{
    printf("%lld\n", (long long)value);
}

void bbgm_print_carb(Carb value)
{
    printf("%f\n", value);
}

void bbgm_print_kcal(Kcal value)
{
    if (value != NULL)
    {
        printf("%s\n", value);
    }
    else
    {
        printf("(null)\n");
    }
}

void bbgm_print_yum(Yum value)
{
    printf("%s\n", value ? "yum" : "yuck");
}

void bbgm_print_cal(Cal value)
{
    printf("%c\n", value);
}

// ==================== MEMORY MANAGEMENT ====================

void *bbgm_alloc(size_t size)
{
    void *ptr = malloc(size);
    if (ptr == NULL)
    {
        fprintf(stderr, "BubblGum Runtime Error: Memory allocation failed\n");
        exit(1);
    }
    return ptr;
}

void bbgm_free(void *ptr)
{
    if (ptr != NULL)
    {
        free(ptr);
    }
}

// ==================== ARRAY OPERATIONS ====================

// Create a new array
BBGMArray *bbgm_array_new(size_t element_size, Sugar length)
{
    BBGMArray *arr = (BBGMArray *)bbgm_alloc(sizeof(BBGMArray));
    arr->length = length;
    arr->element_size = element_size;
    arr->data = bbgm_alloc(element_size * length);
    return arr;
}

// Free an array
void bbgm_array_free(BBGMArray *arr)
{
    if (arr != NULL)
    {
        bbgm_free(arr->data);
        bbgm_free(arr);
    }
}

// Get array element at index
void *bbgm_array_get(BBGMArray *arr, Sugar index)
{
    if (index < 0 || index >= arr->length)
    {
        fprintf(stderr, "BubblGum Runtime Error: Array index %lld out of bounds (length: %lld)\n",
                (long long)index, (long long)arr->length);
        exit(1);
    }
    return (char *)arr->data + (index * arr->element_size);
}

// Set array element at index
void bbgm_array_set(BBGMArray *arr, Sugar index, void *value)
{
    if (index < 0 || index >= arr->length)
    {
        fprintf(stderr, "BubblGum Runtime Error: Array index %lld out of bounds (length: %lld)\n",
                (long long)index, (long long)arr->length);
        exit(1);
    }
    void *dest = (char *)arr->data + (index * arr->element_size);
    memcpy(dest, value, arr->element_size);
}

// ==================== STRING OPERATIONS ====================

// Create a new string
Kcal bbgm_string_new(const char *str)
{
    if (str == NULL)
    {
        return NULL;
    }
    size_t len = strlen(str);
    char *new_str = (char *)bbgm_alloc(len + 1);
    strcpy(new_str, str);
    return new_str;
}

// Concatenate two strings
Kcal bbgm_string_concat(Kcal str1, Kcal str2)
{
    if (str1 == NULL)
        str1 = "";
    if (str2 == NULL)
        str2 = "";

    size_t len1 = strlen(str1);
    size_t len2 = strlen(str2);
    char *result = (char *)bbgm_alloc(len1 + len2 + 1);

    strcpy(result, str1);
    strcat(result, str2);

    return result;
}

// Get string length
Sugar bbgm_string_length(Kcal str)
{
    if (str == NULL)
    {
        return 0;
    }
    return (Sugar)strlen(str);
}

// Compare two strings
Yum bbgm_string_equals(Kcal str1, Kcal str2)
{
    if (str1 == NULL && str2 == NULL)
        return true;
    if (str1 == NULL || str2 == NULL)
        return false;
    return strcmp(str1, str2) == 0;
}

// ==================== MATH OPERATIONS ====================

// Power function for integers
Sugar bbgm_pow_sugar(Sugar base, Sugar exponent)
{
    if (exponent < 0)
    {
        fprintf(stderr, "BubblGum Runtime Error: Negative exponent not supported for integer power\n");
        exit(1);
    }

    Sugar result = 1;
    for (Sugar i = 0; i < exponent; i++)
    {
        result *= base;
    }
    return result;
}

// Power function for floats
Carb bbgm_pow_carb(Carb base, Carb exponent)
{
    return pow(base, exponent);
}

// ==================== TYPE CONVERSION ====================

// Convert sugar to carb
Carb bbgm_sugar_to_carb(Sugar value)
{
    return (Carb)value;
}

// Convert carb to sugar
Sugar bbgm_carb_to_sugar(Carb value)
{
    return (Sugar)value;
}

// Convert sugar to kcal (string)
Kcal bbgm_sugar_to_kcal(Sugar value)
{
    char buffer[32];
    snprintf(buffer, sizeof(buffer), "%lld", (long long)value);
    return bbgm_string_new(buffer);
}

// Convert carb to kcal (string)
Kcal bbgm_carb_to_kcal(Carb value)
{
    char buffer[32];
    snprintf(buffer, sizeof(buffer), "%f", value);
    return bbgm_string_new(buffer);
}

// ==================== ENTRY POINT ====================

// Main entry point (calls user's Main recipe if it exists)
int main(int argc, char **argv)
{
    // The LLVM generated code will define a Main function if one exists
    // We'll link against it
    return 0;
}
