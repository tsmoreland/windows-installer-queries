//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
module;

#include <algorithm>
#include <exception>
#include <optional>
#include <sstream>
#include <stdexcept>
#include <string_view>

#include <Windows.h>

#pragma comment(lib, "rpcrt4.lib")

export module Guid;

namespace win32 {

    template <typename TCHAR, typename CONVERTER>
    GUID from_string(TCHAR* value, CONVERTER converter) {
        GUID output;
        if (RPC_S_OK != converter(value, &output)) {
            throw std::invalid_argument("invalid format");
        }

        return output;
    }


    export class guid final {
        GUID value_{};

    public:
        explicit guid() {}
        explicit guid(GUID const& value) noexcept : value_{value} {}
        explicit guid(char const* value) {
            if (value == nullptr) {
                throw std::invalid_argument("value is null");
            }

            using char_type = std::remove_pointer<RPC_CSTR>::type;
            char_type buffer[64]{};
            memcpy_s(buffer, sizeof(buffer), value, strlen(value) * sizeof(char_type));
            buffer[63] = '\0';

            value_ = from_string(buffer, [](char_type* value, UUID* out) { return UuidFromStringA(value, out); });
        }
        explicit guid(wchar_t const* value) {
            if (value == nullptr) {
                throw std::invalid_argument("value is null");
            }

            using char_type = std::remove_pointer<RPC_WSTR>::type;
            char_type buffer[64]{};
            memcpy_s(buffer, sizeof(buffer), value, wcslen(value) * sizeof(char_type));
            buffer[63] = '\0';

            value_ = from_string(buffer, [](char_type* value, UUID* out) { return UuidFromStringW(value, out); });
        }

        static std::optional<guid> try_parse(char const* value) {
            try {
                return {guid(value)};

            } catch (std::exception const&) {
                return std::nullopt;
            }
        }
        static std::optional<guid> try_parse(wchar_t const* value) {
            try {
                return {guid(value)};

            } catch (std::exception const&) {
                return std::nullopt;
            }
        }

        guid const& zero() {
            static guid empty(GUID{});
            return empty;
        }

        GUID const& get() const noexcept {
            return value_;
        }

        void swap(guid& other) noexcept {
            std::swap(value_, other.value_);
        }

        friend bool operator==(guid const& left, guid const& right) noexcept;
        friend bool operator!=(guid const& left, guid const& right) noexcept;

        friend bool operator==(GUID const& left, guid const& right) noexcept;
        friend bool operator!=(GUID const& left, guid const& right) noexcept;

        friend bool operator==(guid const& left, GUID const& right) noexcept;
        friend bool operator!=(guid const& left, GUID const& right) noexcept;

        friend std::ostream& operator<<(std::ostream& os, const guid& obj);
    };

    export std::wstring to_wstring(guid const& uid) {
        constexpr int null_terminator_size = 1;
        constexpr auto guid_length         = static_cast<std::wstring::size_type>(36);

        constexpr auto size = std::size(L"{00000000-0000-0000-0000-000000000000}") + null_terminator_size;
        wchar_t buffer[size];
        if (StringFromGUID2(uid.get(), buffer, static_cast<int>(size)) == 0)
            return L"********-****-****-****-************";

        return std::wstring(buffer + 1, guid_length);
    }
    export std::string to_string(guid const& uid) {
        auto value = to_wstring(uid);
        std::string value_a{};

        std::transform(std::begin(value), std::end(value), std::back_inserter(value_a), [](auto const& wide_char) {
            const wchar_t ascii_upper_bound = 127;
            const wchar_t ascii_lower_bound = 0;
            return wide_char > ascii_upper_bound || wide_char < ascii_lower_bound ? '?' : static_cast<char>(wide_char);
        });
        return std::move(value_a);
    }
    export void swap(guid& left, guid& right) noexcept {
        left.swap(right);
    }

    export std::wstring to_registry_wstring(guid const& uid) {
        constexpr int null_terminator_size = 1;
        constexpr auto guid_length         = static_cast<std::wstring::size_type>(36);

        constexpr auto size = std::size(L"{00000000-0000-0000-0000-000000000000}") + null_terminator_size;
        wchar_t buffer[size];
        if (StringFromGUID2(uid.get(), buffer, static_cast<int>(size)) == 0)
            return L"********-****-****-****-************";

        return {buffer, guid_length};
    }
    export std::string to_registry_string(guid const& uid) {
        auto value = to_registry_wstring(uid);
        std::string value_a{};

        std::transform(std::begin(value), std::end(value), std::back_inserter(value_a), [](auto const& wide_char) {
            const wchar_t ascii_upper_bound = 127;
            const wchar_t ascii_lower_bound = 0;
            return wide_char > ascii_upper_bound || wide_char < ascii_lower_bound ? '?' : static_cast<char>(wide_char);
        });
        return std::move(value_a);
    }

    export bool operator==(guid const& left, guid const& right) noexcept {
        return left.value_ == right.value_;
    }

    export bool operator!=(guid const& left, guid const& right) noexcept {
        return left.value_ != right.value_;
    }

    export bool operator==(GUID const& left, guid const& right) noexcept {
        return left == right.value_;
    }

    export bool operator!=(GUID const& left, guid const& right) noexcept {
        return left != right.value_;
    }
    export bool operator==(guid const& left, GUID const& right) noexcept {
        return left.value_ == right;
    }

    export bool operator!=(guid const& left, GUID const& right) noexcept {
        return left.value_ != right;
    }

    export std::ostream& operator<<(std::ostream& os, const guid& obj) {
        return os << to_string(obj).c_str();
    }

    export [[nodiscard]] guid new_guid() noexcept {
        return guid();
    }

} // namespace system
