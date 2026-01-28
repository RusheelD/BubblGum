#ifndef BUBBLEGUM_RUNTIME_H
#define BUBBLEGUM_RUNTIME_H

#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>
#include <math.h>

// ==================== TYPE DEFINITIONS ====================

// BubblGum primitive types
typedef int64_t Sugar; // Integer
typedef double Carb;   // Floating point
typedef char Cal;      // Character
typedef char *Kcal;    // String
typedef bool Yum;      // Boolean

// Array structure
typedef struct
{
    Sugar length;
    size_t element_size;
    void *data;
} BBGMArray;

// ==================== PRINT FUNCTIONS ====================

void bbgm_print_sugar(Sugar value);
void bbgm_print_carb(Carb value);
void bbgm_print_kcal(Kcal value);
void bbgm_print_yum(Yum value);
void bbgm_print_cal(Cal value);

// ==================== MEMORY MANAGEMENT ====================

void *bbgm_alloc(size_t size);
void bbgm_free(void *ptr);

// ==================== ARRAY OPERATIONS ====================

BBGMArray *bbgm_array_new(size_t element_size, Sugar length);
void bbgm_array_free(BBGMArray *arr);
void *bbgm_array_get(BBGMArray *arr, Sugar index);
void bbgm_array_set(BBGMArray *arr, Sugar index, void *value);

// ==================== STRING OPERATIONS ====================

Kcal bbgm_string_new(const char *str);
Kcal bbgm_string_concat(Kcal str1, Kcal str2);
Sugar bbgm_string_length(Kcal str);
Yum bbgm_string_equals(Kcal str1, Kcal str2);

// ==================== MATH OPERATIONS ====================

Sugar bbgm_pow_sugar(Sugar base, Sugar exponent);
Carb bbgm_pow_carb(Carb base, Carb exponent);

// ==================== TYPE CONVERSION ====================

Carb bbgm_sugar_to_carb(Sugar value);
Sugar bbgm_carb_to_sugar(Carb value);
Kcal bbgm_sugar_to_kcal(Sugar value);
Kcal bbgm_carb_to_kcal(Carb value);

#endif // BUBBLEGUM_RUNTIME_H
